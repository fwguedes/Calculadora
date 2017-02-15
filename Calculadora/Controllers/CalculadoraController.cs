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
#warning Usar interface
#warning Usar injeção de dependência no contrutor or por Mapping IoC com Simple Injector - Ver com o Daniel
            ClientRabbitCalculadora calc = new ClientRabbitCalculadora();
            var resposta = calc.Call(conta);
            return resposta;            
        }
    }
}
