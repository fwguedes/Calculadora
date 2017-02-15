using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace RabbitCalculadora
{
    class RabbitCalculadora
    {
        static void Main(string[] args)
        {
            //Criando a conexao com o rabbit
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using ( var conexao = factory.CreateConnection())
                //Criando o  canal
                using (var canal = conexao.CreateModel())
                {
    
                    //Criando a fila
                    canal.QueueDeclare(
                        queue: "calcFila",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    canal.BasicQos(0, 1, false);
                    
                    //Manipulador de eventos
                    var consumer = new EventingBasicConsumer(canal);

                canal.BasicConsume(
                                    queue: "calcFila",
                                    noAck: false,
                                    consumer: consumer
                                    );

                Console.WriteLine("Ouvindo : ...");
                
                    //Evento de mensagem recebida
                    consumer.Received += (model, ea) =>
                    {
                        
                        //Resposta ao cliente
                        string response = null;
                        //Recuperando dados da requisicao
                        var body = ea.Body;
                        var props = ea.BasicProperties;
                        var replyProps = canal.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;
                        
                        try
                        {
                            var message = Encoding.UTF8.GetString(body);
                            string[] numeros = message.Split(';');
                            Console.WriteLine("Recebido : {0} {1} {2}", numeros[0], numeros[2], numeros[1]);
                            
                            if (numeros[2] != "+" && numeros[2] != "-" && numeros[2] != "*" && numeros[2] != "/")
                            {
                                response = "Operacao Invalida";
                            }else
                            {
                                response = Calcular(numeros[2], Convert.ToDouble(numeros[0]), Convert.ToDouble(numeros[1])).ToString();
                            }

                            

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Erro : {0}", e.Message);
                            response = "0";
                        }
                        finally
                        {
                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            canal.BasicPublish(exchange: "",
                                                routingKey: props.ReplyTo,
                                                basicProperties: replyProps,
                                                body: responseBytes);
                            canal.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                            Console.WriteLine("Enviado : {0}", response);
                        }

                    };
           
                Console.WriteLine("Enter para sair");
                Console.ReadLine();
            }
        }

        private static double Calcular(string operacao,double numero1, double numero2)
        {
            switch (operacao)
            {
                case "+": return numero1 + numero2; 
                case "-": return numero1 - numero2; 
                case "*": return numero1 * numero2; 
                case "/": return numero1 / numero2; 
                default: return 0;
            }
        }

    }
}
