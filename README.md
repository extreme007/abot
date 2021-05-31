#Create Model
Scaffold-DbContext "Data Source=.;User Id =sa;Password=123456;Initial Catalog=tintin247.com;Integrated Security=True;MultipleActiveResultSets=True" Microsoft.EntityFrameworkCore.SqlServer -o Models
##Update Model
Scaffold-DbContext "Data Source=.;User Id =sa;Password=123456;Initial Catalog=tintin247.com;Integrated Security=True;MultipleActiveResultSets=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -force