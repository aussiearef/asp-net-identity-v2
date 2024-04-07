
# ASP.NET Core Identity Management & Security

This repository belongs to the below online course about ASP.NET Core's built-in identity and access management system. 

[![ASP.NET Core 2 Security and Identity Management](https://img-c.udemycdn.com/course/750x422/1628048_aafb_14.jpg)](https://www.udemy.com/course/aspnet-core-2-security-and-identity-management-with-c/?referralCode=1AA7034B27B8AAA89324)


The course also covers OWASP and WAF, including an introduction to third-party WAF systems commonly  used in the industry.


## Getting Started

Please follow the below instructions and steps for each project:

1. Create an empty database in SQL Server. The database name used in the course is AspnetIdentityV2. If you name your database something else, please update the connection string in appSettings.json.
2. Update the appSettings.json with the correct details of the SQL Server database.
3. Update the appSettings.json with the correct details of the SMTP server when applicable.
4. Follow the instructions provided in the course and create the database objects using Entity Framework Migrations:
  ```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

[View the course here](https://www.udemy.com/course/aspnet-core-2-security-and-identity-management-with-c/?referralCode=1AA7034B27B8AAA89324)

## More Free Courses on YouTube

[![YouTube](https://img.shields.io/badge/YouTube-Subscribe-red?style=flat&logo=youtube)](http://www.youtube.com/@FreeTechnologyLectures)

Subscribe to the Free Technology and Technology Management Courses channel for free lectures about Coding, DevOps, and Technology Management. [Here is the link to the YouTube channel](http://www.youtube.com/@FreeTechnologyLectures).



