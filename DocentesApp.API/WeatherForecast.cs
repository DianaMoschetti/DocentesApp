namespace DocentesApp.API
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}

/*
 * 
 * Referencias recomendadas:

✅ DocentesApp.Application

✅ DocentesApp.Data (para registrar DbContext/DI)

✅ DocentesEFCore.Domain (opcional; idealmente el API habla con DTOs, pero no pasa nada si lo referencia)



 * */