using Microsoft.EntityFrameworkCore;

namespace VkBot.Data
{
	public class SqlServerDataBase: DataBaseContext
	{
		public SqlServerDataBase(string connectionString) : 
			base(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options)
		{
		}
	}
}