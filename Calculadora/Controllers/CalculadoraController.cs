using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RabbitCalculadora;
using Calculadora.Models;

namespace Calculadora.Controllers
{
    public class CalculadoraController : ApiController
    {                
        public object Post([FromBody] ContaModel conta)
        {
            ClientRabbitCalculadora calc = new ClientRabbitCalculadora();
            var resposta = calc.Call(conta);
            return resposta;            
        }
    }
}
