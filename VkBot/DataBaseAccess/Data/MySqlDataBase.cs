using Microsoft.EntityFrameworkCore;

namespace DataBaseAccess.Data
{
	public class MySqlDataBase: DataBaseContext
	{
		public MySqlDataBase(string connectionString) : 
			base(new DbContextOptionsBuilder().UseMySql(connectionString).Options)
		{
		}
	}
}