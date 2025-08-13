namespace Kafka.Client.Producer;

public interface IKafkaProducer<in TEvent> where TEvent : class
{
	Task ProduceAsync(TEvent @event);
}
