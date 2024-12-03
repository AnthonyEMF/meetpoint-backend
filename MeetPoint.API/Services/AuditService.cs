using MeetPoint.API.Services.Interfaces;

namespace MeetPoint.API.Services
{
	public class AuditService : IAuditService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuditService(
			IHttpContextAccessor httpContextAccessor
			)
		{
			this._httpContextAccessor = httpContextAccessor;
		}

		public string GetUserId()
		{
			var idClaim = _httpContextAccessor.HttpContext
				.User.Claims.Where(x => x.Type == "UserId").FirstOrDefault();

			return idClaim.Value;
		}

		// Activar para cargar el Seed de Datos
		//public string GetUserId()
		//{
		//	return "2a373bd7-1829-4bb4-abb7-19da4257891d";
		//}
	}
}
