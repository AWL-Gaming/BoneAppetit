using System.Collections.Generic;
using UnityEngine;

namespace Boneappetit;

public static class RandomJuliaChildPhrase
{
	private static readonly List<string> PhrasesList = new List<string>
	{
		"No one is born a great cook, one learns by doing. - Julia Child", "You don’t have to cook fancy or complicated masterpieces, just good food from fresh ingredients. - Julia Child", "People who love to eat are always the best people. - Julia Child", "Cooking well doesn’t mean cooking fancy. - Julia Child", "The only time to eat diet food is while you’re waiting for the steak to cook. - Julia Child", "Until I discovered cooking, I was never really interested in anything. - Julia Child", "You are the butter to my bread, and the breath to my life. - Julia Child", "Fat gives things flavor. - Julia Child", "If you’re afraid of butter, use cream. - Julia Child", "I was 32 when I started cooking; up until then, I just ate. - Julia Child",
		"A party without cake is just a meeting. - Julia Child", "The only real stumbling block is fear of failure. In cooking you’ve got to have a what-the-hell attitude. - Julia Child", "In France, cooking is a serious art form and a national sport. - Julia Child", "You are the BOSS of that dough. - Julia Child", "with enough butter anything is good. - Julia Child", "Usually, one’s cooking is better than one thinks it is. - Julia Child"
	};

	public static string GetRandomPhrase()
	{
		return PhrasesList[Random.Range(0, PhrasesList.Count - 1)];
	}
}
