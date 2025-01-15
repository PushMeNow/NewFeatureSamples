namespace CodeStyle.Samples;

public class Sample
{
	public void Test()
	{
		var array = new int[] { 1, 2, 3 };
		array.Where(q => q % 2 == 0)
		     .Select(q => new
		     {
			     First = q.ToString(),
			     Second = q.ToString(),
			     Third = q.ToString(),
			     Fourth = q.ToString(),
			     Fifth = q.ToString(),
			     Sixth = q.ToString()
		     })
		     .ToList()
		     .ForEach(Console.WriteLine);
	}
}
