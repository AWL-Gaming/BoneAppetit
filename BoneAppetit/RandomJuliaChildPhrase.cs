using System.Collections.Generic;
using UnityEngine;

namespace Boneappetit;

internal static class RandomJuliaChildPhrase
{
    private static readonly List<string> Phrases = new List<string>
    {
        "The only time to eat diet food is while you're waiting for the steak to cook.",
        "If you're afraid of butter, use cream.",
        "A party without cake is just a meeting.",
        "Everything in moderation... including moderation.",
        "With enough butter, anything is good.",
        "People who love to eat are always the best people.",
        "No matter what happens in the kitchen, never apologize.",
        "Always remember: If you're alone in the kitchen and you drop the lamb, you can always just pick it up. Who's going to know?",
        "You don't have to cook fancy or complicated masterpieces, just good food from fresh ingredients.",
        "This is my invariable advice to people: Learn how to cook, try new recipes, learn from your mistakes, be fearless, and above all have fun!",
        "The best way to execute French cooking is to get good and loaded and whack the hell out of a chicken.",
        "It's so beautifully arranged on the plate, you know someone's fingers have been all over it.",
        "Just like becoming an expert in wine, you learn by drinking it, the best you can afford.",
        "The secret of a happy marriage is finding the right person. You know they're right if you love to be with them all the time.",
        "I think every woman should have a blowtorch.",
        "You'll never know everything about anything, especially something you love."
    };

    internal static string GetRandomPhrase()
    {
        return Phrases[Random.Range(0, Phrases.Count)];
    }
}
