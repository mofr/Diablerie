using System;

[Flags]
public enum CollisionLayer
{
    None = 0,
    Walk = 1, // both player and mercenary
    Light = 2, // and line of sight
    Jump = 4, // and teleport probably
    PlayerWalk = 8,
    LightOnly = 32, // but not line of sight
    Item = 64, // used by Diablerie because it was free
}