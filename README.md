<h1 align=center>Blind Trust</h1>

A multiplayer, cooperative, 3D platformer game, in which 2 players control the same agent: if one of them moves, the other one cannot. Moreover, one player can only see white platforms, while the other player only the black ones. They have to coordinate movements in order to reach the last (red) platform.

> [!NOTE]\
> This is a very small project mainly done for the sake of experimentation in multiplayer games in Unity.

Check it out on [itch.io](https://aprecoma.itch.io/blind-trust).

<h2 align=center>Technologies</h2>

### Unity3D

[Unity](https://unity.com/) is used as Game Engine (version 6000.3.13f1), with the addition of some multiplayer packages.

### Netcode for GameObjects

[Netcode for GameObject](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.13/manual/index.html) is adopted as multiplayer infrastructure (version 2.13.1). One player must act as the host (server + client), while the other one as a client.

### Unity Relay Service

[Unity Relay Service](https://docs.unity.com/en-us/relay) is needed to ensure a remote connection among the players: the host sets up the relay service, receiving a code that the other client has to use to join the server. Version 2.3.0 of Multiplayer Services is installed.

<h2 align=center>Agent coordination</h2>

### Movement

Once a player press a key in order to move the agent, a simple handshake protocol is performed as follows.

- $\textcolor{blue}{\textbf{\textsf{Blue}}}$ - movement request

- $\textcolor{red}{\textbf{\textsf{Red}}}$ - movement request ACK

- $\textcolor{green}{\textbf{\textsf{Green}}}$ - change ownership and set the `movementHandler` NetworkVariable (to the server)

<div align=center>
  <img src="./Resources/movement-diagram.png"></img>
</div>

In case players send the request at the same time, the host has higher priority and will drop the client's request.

When the handler release the movement, it just notifies the server to reset the `movementHandler` NetworkVariable to `None`.

If the player was too fast to press and release a key to move the agent, it was possible that the release protocol completed before the movement protocol (due to network latency). To avoid that, a safety check is introduced, which does not allow the release if the request has not yet been concluded.

### Jump

The movement is managed via the Unity `CharacterController` component, hence the vertical velocity is manually calculated every frame. Once a player decides to jump, it sends the request to the server and the "jump message" is broadcasted, as shown in the following diagram.

- $\textcolor{blue}{\textbf{\textsf{Blue}}}$ - jump request (to the server)

- $\textcolor{green}{\textbf{\textsf{Green}}}$ - change ownership

- $\textcolor{orange}{\textbf{\textsf{Orange}}}$ - broadcast jump message

<div align=center>
  <img src="./Resources/jump-diagram.png"></img>
</div>

Since the jump is instantaneous, no ACK is needed (at worst the same velocity will be overwritten). Moreover, the `movementHandler` NetworkVariable is not changed for the same reason, and so one player may jump while the other player move the agent in the air.

<h2 align=center>Player disconnection</h2>

The disconnection system is managed leveraging `NetworkManager` methods and events, as shown in the following diagram.

- $\textcolor{green}{\textbf{\textsf{Green}}}$ - local disconnection event

- $\textcolor{blue}{\textbf{\textsf{Blue}}}$ - agent despawn request (to the server)

- $\textcolor{red}{\textbf{\textsf{Red}}}$ - actual client shutdown using `NetworkManager.Shutdown()` method

- $\textcolor{orange}{\textbf{\textsf{Orange}}}$ - Netcode connection event received of type `ClientDisconnected`

<div align=center>
  <img src="./Resources/disconnection-diagram.png"></img>
</div>

Once the Netcode disconnection event is received, the game is reset and the other player that did not generate the event calls the `NetworkManager.Shutdown()` method for a proper disconnection.

In case a third player tries to join, the connection is not approved by the host and the new player disconnects immediately. Since a `ClientDisconnected` event is received, the following 2 checks are performed.

1. Disconnected client ID equal to 2 means a third client has just been disconnected, so it is ignored.

2. Disconnected client ID equal to 0 is given to the disconnected client, as the host, but `NetworkManager.IsServer` is false, so perform the reset.
