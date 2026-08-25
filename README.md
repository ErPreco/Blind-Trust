<h1 align=center>Blind Trust</h1>

A multiplayer, cooperative, 3D platformer game, in which 2 players control the same agent: if one of them moves, the other one cannot. Moreover, one player can only see white platforms, while the other player only the black ones. They have to coordinate movements in order to reach the last (red) platform.

> [!NOTE]\
> This is a very small project mainly done for the sake of experimentation in multiplayer games in Unity.

<h2 align=center>Technologies</h2>

### Unity3D

[Unity](https://unity.com/) is used as Game Engine (version 6000.3.11f), with the addition of some multiplayer packages.

### Netcode for GameObjects

[Netcode for GameObject](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.13/manual/index.html) is adopted as multiplayer infrastructure. One player must act as the host (server + client), while the other one as a client.

### Unity Relay Service

[Unity Relay Service](https://docs.unity.com/en-us/relay) is needed to ensure a remote connection among the players: the host sets up the relay service, receiving a code that the other client has to use to join the server.
