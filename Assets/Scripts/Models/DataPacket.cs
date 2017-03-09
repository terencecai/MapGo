using System;
using System.Collections.Generic;
[Serializable]
public class DataPacket {

	public List<User> nearest_players = new List<User>();
	public List<Tavern> nearest_taverns = new List<Tavern>();
	public List<Depot> nearest_depots = new List<Depot>();
}
