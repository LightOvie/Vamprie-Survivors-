using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

	float currentEventCooldown = 0;

	public EVentData[] events;

	[Tooltip("How long to wait before this beomes active.")]
	public float firstTriggerDelay = 180f;

	[Tooltip("How long to wait before each event.")]
	public float triggerInterval = 30f;

	public static EventManager instance;

	[System.Serializable]
	public class Event
	{
		public EVentData data;
		public float duration, cooldown = 0;
	}

	List<Event> runningEvents = new List<Event>();

	PlayerStats[] allPlayers;
	// Start is called before the first frame update
	void Start()
	{
		instance = this;

		currentEventCooldown = firstTriggerDelay > 0 ? firstTriggerDelay : triggerInterval;
		allPlayers = FindObjectsOfType<PlayerStats>();
	}

	// Update is called once per frame
	void Update()
	{
		currentEventCooldown -= Time.deltaTime;
		if (currentEventCooldown <= 0)
		{
			EVentData eVent = GetRandomEvent();

			if (eVent && eVent.CheckIfWillHappen(allPlayers[Random.Range(0, allPlayers.Length)]))
			{
				runningEvents.Add(new Event
				{
					data = eVent,
					duration = eVent.duration
				});

			}
			currentEventCooldown = triggerInterval;
		}

		List<Event> toRemove = new List<Event>();

		foreach (Event e in runningEvents)
		{
			e.duration -= Time.deltaTime;
			if (e.duration <= 0)
			{
				toRemove.Add(e);
				continue;
			}

			e.cooldown -= Time.deltaTime;
			if (e.cooldown<=0)
			{
				e.data.Activate(allPlayers[Random.Range(0, allPlayers.Length)]);
				e.cooldown = e.data.GetSpawnInterval();
			}
		}

		foreach (Event e in toRemove)
		{
			runningEvents.Remove(e);
		}
	}

	public EVentData GetRandomEvent()
	{
		if (events.Length<=0)
		{
			return null;
		}

		List<EVentData> possibleEvents= new List<EVentData>(events);

		EVentData result = possibleEvents[Random.Range(0, possibleEvents.Count)];

		while (!result.IsActive())
		{
			possibleEvents.Remove(result);
			if (possibleEvents.Count>0)
			{
				result = events[Random.Range(0, possibleEvents.Count)];

			}
			else
			{
				return null;
			}
		}

		return result;
	}
}
