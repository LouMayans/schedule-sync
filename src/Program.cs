using System;
using System.Collections;
using System.Collections.Generic;

namespace ScheduleSync
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Person person1 = new Person();
            Person person2 = new Person();

            person1.dayDates.Add(new Date(0));
            person2.dayDates.Add(new Date(0));

            person1.dayDates[0].events.Add(new Event(8, 10));
            person1.dayDates[0].events.Add(new Event(14,16));
            person1.dayDates[0].events.Add(new Event(18, 19));

            person2.dayDates[0].events.Add(new Event(11, 12));
            person2.dayDates[0].events.Add(new Event(15, 16));
            person2.dayDates[0].events.Add(new Event(19, 21));

            person1.dayDates.Add(new Date(1));
            person2.dayDates.Add(new Date(1));

            person1.dayDates[1].events.Add(new Event(8, 10));
            person1.dayDates[1].events.Add(new Event(14, 16));

            person2.dayDates[1].events.Add(new Event(11, 12));
            person2.dayDates[1].events.Add(new Event(13, 14));
            person2.dayDates[1].events.Add(new Event(19, 21));
            person2.dayDates[1].events.Add(new Event(22, 24));

            Person FreeTimeGuy = getFreeTime(person1, person2);
        }

        public static Person getFreeTime(Person person1, Person person2)
        {
            Person freeTime = new Person();

            Person current;
            if (person1.dayDates.Count < person2.dayDates.Count)
                current = person1;
            else
                current = person2;

            for (int i = 0; i < current.dayDates.Count; i++)
            {
                freeTime.dayDates.Add(getBusyTimes(person1.dayDates[i], person2.dayDates[i]));
            }

            return freeTime;
        }

        public static Date getBusyTimes(Date person1, Date person2)
        {
            Date busyTime = new Date(person1._day);
            int person1Count = 0;
            int person2Count = 0;
            while(! ((person1.events.Count == person1Count) && (person2.events.Count == person2Count)) )
            {
                if ((person1.events.Count == person1Count) && (person2.events.Count == person2Count))
                    break;
                if(person1.events.Count == person1Count)
                {
                    busyTime.events.Add(new Event(person2.events[person2Count].startTime, person2.events[person2Count].endTime));
                    ++person2Count;
                }
                else if (person2.events.Count == person1Count)
                {
                    busyTime.events.Add(new Event(person2.events[person2Count].startTime, person2.events[person2Count].endTime));
                    ++person2Count;
                }
                else if(person1.events[person1Count].startTime > person2.events[person2Count].startTime)
                {
                    //person1  starts is the latest
                    if(person1.events[person1Count].endTime < person2.events[person2Count].endTime)
                    {
                        //person1 end is the earliest
                        busyTime.events.Add(new Event(person1.events[person1Count].startTime, person1.events[person1Count].endTime));
                        ++person1Count;
                    }
                    else if(person1.events[person1Count].endTime > person2.events[person2Count].endTime)
                    {
                        if (person1.events[person1Count].startTime == person2.events[person2Count].endTime)
                        {
                            //*if the both events are literally next to eachother not colliding but no free time in between.
                            busyTime.events.Add(new Event(person2.events[person2Count].startTime, person1.events[person1Count].endTime));
                            ++person1Count;
                            ++person2Count;
                        }
                        else{
                            //*Person 2 is both earlier in start and end then add person 2 to result and increment
                            busyTime.events.Add(new Event(person2.events[person2Count].startTime, person2.events[person2Count].endTime));
                            ++person2Count;
                        }
                    }
                    else
                    {
                        busyTime.events.Add(new Event(person1.events[person1Count].startTime, person2.events[person2Count].endTime));
                        ++person1Count;
                        ++person2Count;
                    }
                }
                else
                {
                    //person2 starts is the latest
                    if (person1.events[person1Count].endTime > person2.events[person2Count].endTime)
                    {
                        //person1 ends the earliest
                        busyTime.events.Add(new Event(person2.events[person2Count].startTime, person1.events[person1Count].endTime));
                        ++person1Count;
                    }
                    else if (person1.events[person1Count].endTime < person2.events[person2Count].endTime)
                    {
                        if (person1.events[person1Count].endTime == person2.events[person2Count].startTime)
                        {
                            //*if the both events are literally next to eachother not colliding but no free time in between.
                            busyTime.events.Add(new Event(person1.events[person1Count].startTime, person2.events[person1Count].endTime));
                            ++person1Count;
                            ++person2Count;
                        }
                        else
                        {
                            busyTime.events.Add(new Event(person1.events[person1Count].startTime, person1.events[person1Count].endTime));
                            ++person1Count;
                        }
                        //*if person1 event is both start and end date earlier than the other. so just add that time to result then increment
                        
                    }
                    else
                    {
                        busyTime.events.Add(new Event(person1.events[person1Count].startTime, person2.events[person2Count].endTime));
                        ++person2Count;
                        ++person1Count;
                    }
                }
            }

            return busyTime;
        }
    }

    

    public class Person
    {
        public List<Date> dayDates = new List<Date>();
    }
    public class Date
    {
        public Date(Date copy)
        {
            for (int i = 0; i < copy.events.Count; i++)
            {
                events.Add(new Event(copy.events[i].startTime,copy.events[i].endTime));
            }
        }
        public Date(int day)
        {
            _day = day; 
        }
        public int _day;
        public List<Event> events = new List<Event>();
    }
    public class Event
    {
        public Event(float start, float end)
        {
            startTime = start;
            endTime = end;
        }
        public float startTime;
        public float endTime;
    }
}
