Nuevo Proyecto WebApi Core

https://www.learmoreseekmore.com/2022/11/dotnet7-api-crud-efcore.html
dotnet add package Microsoft.EntityFrameworkCore --version 7.0.3
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 7.0.3
https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/templates?tabs=dotnet-core-cli

dotnet tool update --global dotnet-ef
dotnet new install Microsoft.EntityFrameworkCore.Templates --version 7.0.3
dotnet new ef-templates


dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet ef dbcontext scaffold "Data Source=SQL5105.site4now.net;Initial Catalog=db_a5e599_foul;User Id=db_a5e599_foul_admin;Password=Sql2019!;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -d --data-annotations  -o Model  -c  Entities  --context-dir Context --context-namespace FoulNet  -f --use-database-names --no-build --no-pluralize --no-onconfiguring	
