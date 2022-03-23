namespace DevSpector.SDK
{
	public interface IRawDataProvider
	{
        TOut Deserialize<TOut>(string json);
	}
}
