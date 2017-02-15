using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Calculadora.Models;

namespace RabbitCalculadora
{
    class ClientRabbitCalculadora
    {

        private IConnection conexao;
        private IModel canal;
        private string replyQueueName;
        private QueueingBasicConsumer consumer;

        //Construindo o cliente
        public ClientRabbitCalculadora()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            conexao = factory.CreateConnection();
            canal = conexao.CreateModel();
            replyQueueName = canal.QueueDeclare().QueueName;
            consumer = new QueueingBasicConsumer(canal);


            canal.BasicConsume(
                queue: replyQueueName,
                noAck: true,
                consumer: consumer
                );

        }

        public string Call(ContaModel conta)
        {
            //Criando os dados para a  fila 
            var corrID = Guid.NewGuid().ToString();
            var props = canal.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = corrID;
            //Criando a mensagem
            var contaBytes = Encoding.UTF8.GetBytes(conta.Valor1.ToString() + ";" + conta.Valor2.ToString() + ";" + conta.Operacao.ToString());
            //Publicando na fila
            canal.BasicPublish(
                exchange: "",
                routingKey: "calcFila",
                basicProperties: props,
                body: contaBytes);

            while (true)
            {
                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                if(ea.BasicProperties.CorrelationId == corrID)
                {
                    return Encoding.UTF8.GetString(ea.Body);
                }
            }

        }

        public void Close()
        {
            conexao.Close();
        }

    }
}
