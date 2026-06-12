using JobPortal.Database;
  using JobPortal.Features.Admin;
  using JobPortal.Features.Application;
  using JobPortal.Features.Candidate;
  using JobPortal.Features.Interview;
  using JobPortal.Features.JobPosting;
  using JobPortal.Features.Notifications;
  using JobPortal.Features.User.Candidate;
  using JobPortal.Shared.Enums;

  namespace JobPortal.FileStorage;

  internal static class DataStore
  {
      private static FileStorageService<JobSeeker> _seekerStorage = null!;
      private static FileStorageService<Admin1> _adminStorage = null!;
      private static FileStorageService<JobListing> _jobStorage = null!;
      private static FileStorageService<Application1> _appStorage = null!;
      private static FileStorageService<Interview1> _interviewStorage = null!;
      private static FileStorageService<Notification> _notificationStorage = null!;
      private static FileStorageService<Complaint> _complaintStorage = null!;

      public static List<JobSeeker> JobSeekers { get; private set; } = new();
      public static List<Admin1> Admins { get; private set; } = new();
      public static List<JobListing> Jobs { get; private set; } = new();
      public static List<Application1> Applications { get; private set; } = new();
      public static List<Interview1> Interviews { get; private set; } = new();
      public static List<Notification> Notifications { get; private set; } = new();
      public static List<Complaint> Complaints { get; private set; } = new();

      public static Dictionary<string, JobSeeker> JobSeekerIndex { get; private set; } = new();

      public static NotificationService NotificationService { get; private set; } = null!;
      public static InterviewScheduler InterviewScheduler { get; private set; } = null!;

      public static void LoadAll()
      {
          _seekerStorage = new FileStorageService<JobSeeker>("jobseekers.json");
          _adminStorage  = new FileStorageService<Admin1>("admins.json");
          _jobStorage    = new FileStorageService<JobListing>("jobs.json");
          _appStorage    = new FileStorageService<Application1>("applications.json");
          _interviewStorage = new FileStorageService<Interview1>("interviews.json");
          _notificationStorage = new FileStorageService<Notification>("notifications.json");
          _complaintStorage = new FileStorageService<Complaint>("complaints.json");

          NotificationService  = new NotificationService();
          InterviewScheduler   = new InterviewScheduler();

          SeedDataService.SeedIfMissing();

          JobSeekers   = _seekerStorage.LoadData();
          Admins       = _adminStorage.LoadData();
          {
              List<Admin1> uniqueAdmins = new List<Admin1>();
              List<string> seen = new List<string>();
              foreach (Admin1 a in Admins)
              {
                  string emailLower = a.Email.Trim().ToLowerInvariant();
                  if (!seen.Contains(emailLower))
                  {
                      seen.Add(emailLower);
                      uniqueAdmins.Add(a);
                  }
              }
              Admins = uniqueAdmins;
              _adminStorage.SaveData(Admins);
          }
          Jobs         = _jobStorage.LoadData();
          {
              List<JobListing> uniqueJobs = new List<JobListing>();
              List<string> seen = new List<string>();
              foreach (JobListing j in Jobs)
              {
                  string key = $"{j.PostedByUserId}_{j.Title.Trim().ToLowerInvariant()}";
                  if (!seen.Contains(key))
                  {
                      seen.Add(key);
                      uniqueJobs.Add(j);
                  }
              }
              Jobs = uniqueJobs;
              _jobStorage.SaveData(Jobs);
          }
          Applications = _appStorage.LoadData();
          Interviews = _interviewStorage.LoadData();
          Notifications = _notificationStorage.LoadData();
          Complaints = _complaintStorage.LoadData();

          if (DatabaseSync.IsEnabled)
          {
              if (DatabaseSync.IsSeedingRequired())
              {
                  DatabaseSync.SeedDatabaseSynchronously(
                      Admins,
                      JobSeekers,
                      Jobs,
                      Applications,
                      Interviews,
                      Notifications
                  );
              }
              DatabaseSync.SyncNotifications(Notifications);
              DatabaseSync.LoadAllFromDb();
              InterviewScheduler.Reload();
              NotificationService.Reload();
          }

          RebuildIndex();
          SyncJobSeekerCounter();
          InterviewScheduler.Reload();

          // Post-load fixup: promote any INTERVIEW_SCHEDULED candidate whose round
          // counter already equals TotalRounds (stuck state from a previous session).
          FixupStuckHiredStatus();

          bool adminExists = false;
          foreach (Admin1 a in Admins)
          {
              if (a.Email.Trim().ToLowerInvariant() == "admin@jpns.com")
              {
                  adminExists = true;
                  break;
              }
          }
          if (!adminExists)
              SeedDefaultAdmin();
      }


      /// <summary>
      /// On startup, find any application that is still in INTERVIEW_SCHEDULED state
      /// but whose seeker's CurrentRound has already reached TotalRounds.
      /// These are "stuck" — auto-promote them to HIRED so the UI shows the correct state.
      /// </summary>
      private static void FixupStuckHiredStatus()
      {
          bool anyFixed = false;
          foreach (Application1 app in Applications)
          {
              if (app.Status != ApplicationStatus.INTERVIEW_SCHEDULED
                  && app.Status != ApplicationStatus.SHORTLISTED)
                  continue;

              JobSeeker? seeker = FindJobSeekerById(app.JobSeekerId);
              if (seeker == null) continue;

              if (seeker.CurrentRound >= seeker.TotalRounds)
              {
                  try
                  {
                      // Fix BOTH the application status AND the seeker status.
                      app.Status = ApplicationStatus.HIRED;
                      seeker.CurrentRound    = seeker.TotalRounds;
                      seeker.CandidateStatus = "HIRED";
                      anyFixed = true;
                  }
                  catch { /* ignore individual fixup failures */ }
              }
          }
          if (anyFixed)
          {
              _seekerStorage.SaveData(JobSeekers);
              _appStorage.SaveData(Applications);
              DatabaseSync.SyncJobSeekers(JobSeekers);
              DatabaseSync.SyncApplications(Applications);
          }
      }


        public static void SaveJobSeekers()
        {
            _seekerStorage.SaveData(JobSeekers);

            RebuildIndex();

            DatabaseSync.SyncJobSeekers(JobSeekers);
            DatabaseSync.SyncResumes(JobSeekers);
            DatabaseSync.SyncSavedJobs(JobSeekers);
        }

      public static void SaveAdmins()
      {
          _adminStorage.SaveData(Admins);
          DatabaseSync.SyncAdmins(Admins);
      }

      public static void SaveJobs()
      {
          _jobStorage.SaveData(Jobs);
          DatabaseSync.SyncJobs(Jobs);
      }

      public static void SaveApplications()
      {
          _appStorage.SaveData(Applications);
          DatabaseSync.SyncApplications(Applications);
      }

      public static void SaveInterviews()
      {
         _interviewStorage.SaveData(Interviews);
         DatabaseSync.SyncInterviews(Interviews);
      }

      public static void SaveNotifications()
      {
          _notificationStorage.SaveData(Notifications);
          DatabaseSync.SyncNotifications(Notifications);
      }

      public static void SaveComplaints()
      {
          if (_complaintStorage != null)
          {
              _complaintStorage.SaveData(Complaints);
          }
          DatabaseSync.SyncComplaints(Complaints);
      }


      public static JobSeeker? FindJobSeeker(string identifier)
      {
          string id = identifier.Trim().ToLowerInvariant();
          foreach (JobSeeker seeker in JobSeekers)
          {
              if (seeker.Email.ToLowerInvariant() == id ||
                  seeker.SeekerId.ToLowerInvariant() == id ||
                  seeker.Phone == identifier.Trim())
              {
                  return seeker;
              }
          }
          return null;
      }

      public static JobSeeker? FindJobSeekerById(string seekerId)
      {
          foreach (JobSeeker seeker in JobSeekers)
          {
              if (seeker.SeekerId == seekerId)
                  return seeker;
          }
          return null;
      }

      public static Admin1? FindAdmin(string email)
      {
          string e = email.Trim().ToLowerInvariant();
          foreach (Admin1 admin in Admins)
          {
              if (admin.Email.ToLowerInvariant() == e)
                  return admin;
          }
          return null;
      }

      public static JobSeeker? GetJobSeeker(string email)
      {
          if (JobSeekerIndex.TryGetValue(email.ToLowerInvariant(), out var seeker))
              return seeker;
          return null;
      }

      public static JobSeeker? GetJobSeeker(string id, bool byId)
      {
          if (!byId) return GetJobSeeker(id);
          foreach (JobSeeker seeker in JobSeekers)
          {
              if (seeker.SeekerId == id)
                  return seeker;
          }
          return null;
      }


      private static void RebuildIndex()
      {
          JobSeekerIndex = new Dictionary<string, JobSeeker>(StringComparer.OrdinalIgnoreCase);
          foreach (JobSeeker seeker in JobSeekers)
              JobSeekerIndex[seeker.Email.ToLowerInvariant()] = seeker;
      }

      private static void SeedDefaultAdmin()
      {
          var admin = Admin1.CreateSeedAdmin();
          Admins.Add(admin);
          SaveAdmins();
      }

      private static void SyncJobSeekerCounter()
      {
          int maxNum = 0;
          foreach (JobSeeker s in JobSeekers)
          {
              string[] parts = s.SeekerId.Split('-');
              if (parts.Length == 3 && int.TryParse(parts[2], out int num))
                  if (num > maxNum) maxNum = num;
          }
          JobSeeker.SyncCounter(maxNum + 1);
      }
  }
  