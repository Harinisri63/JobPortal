using JobPortal.Shared.Enums;
using JobPortal.Shared.Structs;
using JobPortal.Features.Admin;
using JobPortal.Features.Application;
using JobPortal.Features.Candidate;
using JobPortal.Features.JobPosting;

namespace JobPortal.FileStorage;
internal static class SeedDataService
{
    private static readonly FileStorageService<Admin1>       _adminStorage  = new("admins.json");
    private static readonly FileStorageService<JobSeeker>    _seekerStorage = new("jobseekers.json");
    private static readonly FileStorageService<JobListing>   _jobStorage    = new("jobs.json");
    private static readonly FileStorageService<Application1> _appStorage    = new("applications.json");

    public static void SeedIfMissing()
    {
        if (!_adminStorage.FileExists())
        {
            _adminStorage.SaveData(BuildAdmins());
            Console.WriteLine("[Seed] Created admins.json");
        }

        if (!_seekerStorage.FileExists())
        {
            _seekerStorage.SaveData(BuildJobSeekers());
            Console.WriteLine("[Seed] Created jobseekers.json");
        }

        if (!_jobStorage.FileExists())
        {
            _jobStorage.SaveData(BuildJobs());
            Console.WriteLine("[Seed] Created jobs.json");
        }

        if (!_appStorage.FileExists())
        {
            _appStorage.SaveData(BuildApplications());
            Console.WriteLine("[Seed] Created applications.json");
        }
    }


    private static List<Admin1> BuildAdmins() => new()
    {
        Admin1.CreateSeedAdmin(),   

        Admin1.CreateSeed("ADM002", "Ramesh Iyer",   "ramesh.iyer@jpns.com",
                          "JPNS", "Ramesh@123"),

        Admin1.CreateSeed("ADM003", "Sneha Nair",    "sneha.nair@jpns.com",
                          "JPNS",    "Sneha@123"),
    };

  

    private static List<JobSeeker> BuildJobSeekers() => new()
    {
        JobSeeker.CreateSeed("JSK-2026-0001", "Arun Kumar",      "arun.kumar@example.com",
            "9876543210", "Arun@1234",   "Chennai",
            "C#, .NET, SQL Server, REST APIs",
            "B.E. Computer Science", "2 years"),

        JobSeeker.CreateSeed("JSK-2026-0002", "Priya Sharma",    "priya.sharma@example.com",
            "9123456780", "Priya@1234",  "Bangalore",
            "Python, Django, React, PostgreSQL",
            "M.Sc. Information Technology", "3 years"),

        JobSeeker.CreateSeed("JSK-2026-0003", "Vikram Rajan",    "vikram.rajan@example.com",
            "9988776655", "Vikram@1234", "Hyderabad",
            "Java, Spring Boot, Microservices, Docker",
            "B.Tech Information Technology", "4 years"),

        JobSeeker.CreateSeed("JSK-2026-0004", "Divya Menon",     "divya.menon@example.com",
            "8877665544", "Divya@1234",  "Kochi",
            "UI/UX Design, Figma, Adobe XD, HTML, CSS",
            "B.Des Visual Communication", "2 years"),

        JobSeeker.CreateSeed("JSK-2026-0005", "Suresh Babu",     "suresh.babu@example.com",
            "9765432109", "Suresh@1234", "Chennai",
            "Data Analysis, Python, Power BI, Excel, SQL",
            "B.Sc. Statistics", "1 year"),

        JobSeeker.CreateSeed("JSK-2026-0006", "Meena Krishnan",  "meena.krishnan@example.com",
            "9654321098", "Meena@1234",  "Coimbatore",
            "Android, Kotlin, Java, Firebase",
            "B.E. Electronics and Communication", "2 years"),

        JobSeeker.CreateSeed("JSK-2026-0007", "Arjun Pillai",    "arjun.pillai@example.com",
            "9543210987", "Arjun@1234",  "Trivandrum",
            "DevOps, AWS, Docker, Kubernetes, CI/CD",
            "B.Tech Computer Science", "5 years"),

        JobSeeker.CreateSeed("JSK-2026-0008", "Kavitha Devi",    "kavitha.devi@example.com",
            "9432109876", "Kavitha@1234","Madurai",
            "PHP, Laravel, MySQL, JavaScript, jQuery",
            "B.Sc. Computer Science", "3 years"),

        JobSeeker.CreateSeed("JSK-2026-0009", "Nikhil Reddy",    "nikhil.reddy@example.com",
            "9321098765", "Nikhil@1234", "Hyderabad",
            "Machine Learning, Python, TensorFlow, Pandas, NumPy",
            "M.Tech Artificial Intelligence", "2 years"),

        JobSeeker.CreateSeed("JSK-2026-0010", "Lakshmi Narayanan","lakshmi.n@example.com",
            "9210987654", "Lakshmi@1234","Chennai",
            "QA Testing, Selenium, TestNG, JIRA, Postman",
            "B.E. Computer Science", "3 years"),
    };


    private static List<JobListing> BuildJobs()
    {
        string a1 = "ADM001";
        string a2 = "ADM002";
        string a3 = "ADM003";

        return new List<JobListing>
        {
            JobListing.CreateSeed("JOB-2026-0001", a1,
                "Software Engineer (.NET)",
                "Design, develop and maintain C#/.NET applications. Collaborate with cross-functional teams " +
                "to deliver high-quality software solutions and REST APIs.",
                "JPNS", "Chennai",
                new SalaryRange(6, 12), 2,
                new List<string> { "C#", ".NET", "SQL Server", "REST APIs" },
                isActive: true,  status: JobStatus.APPROVED, postedDaysAgo: 15),

            JobListing.CreateSeed("JOB-2026-0002", a2,
                "Full Stack Developer",
                "Build end-to-end web applications using React and Node.js. Strong problem-solving skills and " +
                "experience with TypeScript and MongoDB required.",
                "JPNS", "Bangalore",
                new SalaryRange(8, 18), 3,
                new List<string> { "React", "Node.js", "MongoDB", "TypeScript" },
                isActive: true, status: JobStatus.APPROVED, postedDaysAgo: 10),

            JobListing.CreateSeed("JOB-2026-0003", a3,
                "Data Analyst",
                "Analyse large datasets, build dashboards and generate business insights. Experience with " +
                "Python or R, and strong SQL skills required.",
                "JPNS", "Hyderabad",
                new SalaryRange(5, 10), 1,
                new List<string> { "Python", "SQL", "Power BI", "Excel" },
                isActive: true, status: JobStatus.APPROVED, postedDaysAgo: 8),

            JobListing.CreateSeed("JOB-2026-0004", a1,
                "Java Backend Developer",
                "Develop microservices using Spring Boot. Experience with Docker, REST APIs and relational " +
                "databases. Knowledge of Kafka is a plus.",
                "JPNS", "Hyderabad",
                new SalaryRange(7, 14), 3,
                new List<string> { "Java", "Spring Boot", "Microservices", "Docker" },
                isActive: true, status: JobStatus.APPROVED, postedDaysAgo: 12),

            JobListing.CreateSeed("JOB-2026-0005", a2,
                "UI/UX Designer",
                "Create wireframes, prototypes and pixel-perfect UI designs. Conduct user research and " +
                "usability testing. Strong portfolio required.",
                "JPNS", "Bangalore",
                new SalaryRange(4, 9), 2,
                new List<string> { "Figma", "Adobe XD", "HTML", "CSS" },
                isActive: true, status: JobStatus.APPROVED, postedDaysAgo: 5),

            JobListing.CreateSeed("JOB-2026-0006", a3,
                "Android Developer",
                "Build and maintain Android applications using Kotlin. Integrate Firebase services and " +
                "REST APIs. Experience with Jetpack Compose preferred.",
                "JPNS", "Kochi",
                new SalaryRange(5, 11), 2,
                new List<string> { "Android", "Kotlin", "Firebase", "REST APIs" },
                isActive: true, status: JobStatus.APPROVED, postedDaysAgo: 7),

            JobListing.CreateSeed("JOB-2026-0007", a1,
                "DevOps Engineer",
                "Manage CI/CD pipelines, cloud infrastructure on AWS and container orchestration with " +
                "Kubernetes. Strong scripting skills required.",
                "JPNS", "Chennai",
                new SalaryRange(10, 20), 5,
                new List<string> { "AWS", "Docker", "Kubernetes", "CI/CD" },
                isActive: true, status: JobStatus.APPROVED, postedDaysAgo: 3),

            JobListing.CreateSeed("JOB-2026-0008", a2,
                "PHP Laravel Developer",
                "Develop and maintain web applications using PHP and Laravel framework. Strong MySQL and " +
                "JavaScript skills required.",
                "JPNS", "Madurai",
                new SalaryRange(3, 7), 2,
                new List<string> { "PHP", "Laravel", "MySQL", "JavaScript" },
                isActive: true, status: JobStatus.APPROVED, postedDaysAgo: 6),

            JobListing.CreateSeed("JOB-2026-0009", a3,
                "Machine Learning Engineer",
                "Build and deploy ML models for production. Experience with deep learning frameworks and " +
                "MLOps practices. Research background preferred.",
                "JPNS", "Hyderabad",
                new SalaryRange(12, 22), 3,
                new List<string> { "Python", "TensorFlow", "Pandas", "MLOps" },
                isActive: true, status: JobStatus.APPROVED, postedDaysAgo: 4),

            JobListing.CreateSeed("JOB-2026-0010", a1,
                "QA Test Engineer",
                "Design and execute test plans for web and API applications. Experience with automation " +
                "frameworks like Selenium and TestNG required.",
                "JPNS", "Chennai",
                new SalaryRange(4, 8), 2,
                new List<string> { "Selenium", "TestNG", "JIRA", "Postman" },
                isActive: false, status: JobStatus.PENDING_APPROVAL, postedDaysAgo: 2),
        };
    }


    private static List<Application1> BuildApplications() => new()
    {
        
        Application1.CreateSeed("APP-001", "JSK-2026-0001", "JOB-2026-0001",
            ApplicationStatus.INTERVIEW_SCHEDULED,
            "Arun Kumar | C#, .NET, SQL | 2 yrs | Chennai",
            "I am passionate about .NET development.", daysAgo: 14),

        Application1.CreateSeed("APP-002", "JSK-2026-0003", "JOB-2026-0001",
            ApplicationStatus.SHORTLISTED,
            "Vikram Rajan | Java, Spring, Docker | 4 yrs | Hyderabad",
            "Eager to transition to .NET ecosystem.", daysAgo: 12),

        Application1.CreateSeed("APP-003", "JSK-2026-0010", "JOB-2026-0001",
            ApplicationStatus.PENDING,
            "Lakshmi Narayanan | QA, Selenium | 3 yrs | Chennai",
            "Looking to move into development.", daysAgo: 5),

        Application1.CreateSeed("APP-004", "JSK-2026-0008", "JOB-2026-0001",
            ApplicationStatus.REJECTED,
            "Kavitha Devi | PHP, Laravel | 3 yrs | Madurai",
            "Interested in backend development.", daysAgo: 11),

        
        Application1.CreateSeed("APP-005", "JSK-2026-0002", "JOB-2026-0002",
            ApplicationStatus.HIRED,
            "Priya Sharma | Python, Django, React | 3 yrs | Bangalore",
            "Full stack is my passion.", daysAgo: 9),

        Application1.CreateSeed("APP-006", "JSK-2026-0004", "JOB-2026-0002",
            ApplicationStatus.INTERVIEW_SCHEDULED,
            "Divya Menon | UI/UX, HTML, CSS | 2 yrs | Kochi",
            "Strong frontend skills to complement the stack.", daysAgo: 8),

        Application1.CreateSeed("APP-007", "JSK-2026-0006", "JOB-2026-0002",
            ApplicationStatus.SHORTLISTED,
            "Meena Krishnan | Android, Kotlin, Firebase | 2 yrs | Coimbatore",
            "Excited to work on full-stack web.", daysAgo: 7),

        
        Application1.CreateSeed("APP-008", "JSK-2026-0005", "JOB-2026-0003",
            ApplicationStatus.INTERVIEW_SCHEDULED,
            "Suresh Babu | Python, Power BI, SQL | 1 yr | Chennai",
            "Data analysis is my core strength.", daysAgo: 7),

        Application1.CreateSeed("APP-009", "JSK-2026-0009", "JOB-2026-0003",
            ApplicationStatus.SHORTLISTED,
            "Nikhil Reddy | ML, Python, Pandas | 2 yrs | Hyderabad",
            "Strong analytical and data skills.", daysAgo: 6),

        Application1.CreateSeed("APP-010", "JSK-2026-0002", "JOB-2026-0003",
            ApplicationStatus.PENDING,
            "Priya Sharma | Python, Django | 3 yrs | Bangalore",
            "Interested in data-focused role.", daysAgo: 4),

        
        Application1.CreateSeed("APP-011", "JSK-2026-0003", "JOB-2026-0004",
            ApplicationStatus.HIRED,
            "Vikram Rajan | Java, Spring Boot, Docker | 4 yrs | Hyderabad",
            "Java microservices is my specialty.", daysAgo: 11),

        Application1.CreateSeed("APP-012", "JSK-2026-0001", "JOB-2026-0004",
            ApplicationStatus.REJECTED,
            "Arun Kumar | C#, .NET | 2 yrs | Chennai",
            "Willing to work with Java stack.", daysAgo: 10),

        
        Application1.CreateSeed("APP-013", "JSK-2026-0004", "JOB-2026-0005",
            ApplicationStatus.INTERVIEW_SCHEDULED,
            "Divya Menon | Figma, Adobe XD, HTML | 2 yrs | Kochi",
            "UI/UX is my core expertise.", daysAgo: 4),

        Application1.CreateSeed("APP-014", "JSK-2026-0002", "JOB-2026-0005",
            ApplicationStatus.PENDING,
            "Priya Sharma | React, HTML, CSS | 3 yrs | Bangalore",
            "Frontend design is something I enjoy.", daysAgo: 3),

        
        Application1.CreateSeed("APP-015", "JSK-2026-0006", "JOB-2026-0006",
            ApplicationStatus.SHORTLISTED,
            "Meena Krishnan | Android, Kotlin, Firebase | 2 yrs | Coimbatore",
            "Native Android development is my passion.", daysAgo: 6),

        Application1.CreateSeed("APP-016", "JSK-2026-0007", "JOB-2026-0006",
            ApplicationStatus.PENDING,
            "Arjun Pillai | DevOps, AWS | 5 yrs | Trivandrum",
            "Interested in mobile platform DevOps.", daysAgo: 2),

        
        Application1.CreateSeed("APP-017", "JSK-2026-0007", "JOB-2026-0007",
            ApplicationStatus.INTERVIEW_SCHEDULED,
            "Arjun Pillai | AWS, Docker, Kubernetes | 5 yrs | Trivandrum",
            "Cloud and DevOps is my domain.", daysAgo: 2),

        Application1.CreateSeed("APP-018", "JSK-2026-0003", "JOB-2026-0007",
            ApplicationStatus.PENDING,
            "Vikram Rajan | Docker, Microservices | 4 yrs | Hyderabad",
            "Keen to move into DevOps role.", daysAgo: 1),

        
        Application1.CreateSeed("APP-019", "JSK-2026-0008", "JOB-2026-0008",
            ApplicationStatus.SHORTLISTED,
            "Kavitha Devi | PHP, Laravel, MySQL | 3 yrs | Madurai",
            "Laravel is my primary framework.", daysAgo: 5),

        
        Application1.CreateSeed("APP-020", "JSK-2026-0009", "JOB-2026-0009",
            ApplicationStatus.INTERVIEW_SCHEDULED,
            "Nikhil Reddy | ML, TensorFlow, Python | 2 yrs | Hyderabad",
            "ML in production is what I specialise in.", daysAgo: 3),
    };
}
