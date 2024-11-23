using System.Diagnostics.CodeAnalysis;

namespace GenericHost;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "DI")]
internal sealed class LoremIpsum
{
	private static readonly IReadOnlyList<string> words = ["lorem", "ipsum", "dolor", "sit", "amet", "integre", "prompta", "quo", "in", "duo", "cu", "sapientem", "gubergren", "vidit", "impedit", "ad", "mel"];
	private readonly Random rnd = new();

	public string GetSentence()
	{
		return string.Join(' ', Enumerable.Range(0, rnd.Next(2, 6)).Select(x => words[rnd.Next(words.Count)]));
	}
}
