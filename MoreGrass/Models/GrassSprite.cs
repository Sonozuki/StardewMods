﻿namespace MoreGrass.Models;

/// <summary>Represents a grass sprite.</summary>
public class GrassSprite
{
    /*********
    ** Properties
    *********/
    /// <summary>The grass sprite.</summary>
    public Texture2D Sprite { get; }

    /// <summary>The locations the sprite is allowed in.</summary>
    public List<string> WhiteListedLocations { get; }

    /// <summary>The locations the sprite isn't allowed in.</summary>
    public List<string> BlackListedLocations { get; }


    /*********
    ** Public Methods
    *********/
    /// <summary>Constructs an instance.</summary>
    /// <param name="sprite">The grass sprite.</param>
    /// <param name="whiteListedLocations">The locations the sprite is allowed in.</param>
    /// <param name="blackListedLocations">The locations the sprite isn't allowed in.</param>
    public GrassSprite(Texture2D sprite, List<string> whiteListedLocations, List<string> blackListedLocations)
    {
        Sprite = sprite;
        WhiteListedLocations = whiteListedLocations ?? [];
        BlackListedLocations = blackListedLocations ?? [];
    }
}
