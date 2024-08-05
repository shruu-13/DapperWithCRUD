using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace DapperWithCRUD.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SuperheroController : ControllerBase
	{
		private readonly IConfiguration _config;

		public SuperheroController(IConfiguration config)
		{
			_config = config;
		}

		[HttpGet]
		public async Task<ActionResult<List<Superhero>>> GetAllSuperheros()
		{

			using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			IEnumerable<Superhero> heroes = await SelectAllHeroes(connection);
			return Ok(heroes);

		}

		

		[HttpGet("{heroId}")]
		public async Task<ActionResult<Superhero>> Gethero(int heroId)
		{

			using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			var hero = await connection.QueryFirstAsync<Superhero>("select * from superhero where id = @Id",
				new {Id = heroId});
			return Ok(hero);

		}

		[HttpPost]
		public async Task<ActionResult<List<Superhero>>> Createheros(Superhero hero)
		{

			using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			await connection.ExecuteAsync("insert into superhero (name, firstname, lastname, place) values (@Name, @FirstName, @LastName, @Place)", hero);
			return Ok(await SelectAllHeroes(connection));

		}

		[HttpPut]
		public async Task<ActionResult<List<Superhero>>> Updateheros(Superhero hero)
		{

			using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			await connection.ExecuteAsync("update superhero set name = @Name, firstname = @FirstName, lastname = @LastName, place = @Place where id = @Id", hero);
			return Ok(await SelectAllHeroes(connection));

		}

		[HttpDelete("{heroId}")] 
		public async Task<ActionResult<List<Superhero>>> Deleteheros(int heroId)
		{

			using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
			await connection.ExecuteAsync("delete from superhero where id = @Id", 
				new {Id = heroId}
				);
			return Ok(await SelectAllHeroes(connection));

		}

		private static async Task<IEnumerable<Superhero>> SelectAllHeroes(SqlConnection connection)
		{
			return await connection.QueryAsync<Superhero>("select * from superhero");
		}
	} 
}
 