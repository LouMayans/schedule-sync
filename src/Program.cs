using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.Services;
namespace ScheduleSync
{
    class Program
    {
        
        [WebMethod()]
        static void Main(string[] args)
        {


            string person1Txt = System.IO.File.ReadAllText(@"C:\Users\Lou\Documents\Stuff\Hackathon\Hackron3000\Algorithm test\ScheduleSync\ScheduleSync\events1.txt");
            string person2Txt = System.IO.File.ReadAllText(@"C:\Users\Lou\Documents\Stuff\Hackathon\Hackron3000\Algorithm test\ScheduleSync\ScheduleSync\events2.txt");

            string[] p1events = person1Txt.Split('\n');
            string[] p2events = person2Txt.Split('\n');

            Person person1 = new Person();
            Person person2 = new Person();

            for (int i = 0; i < p1events.Length; i++)
            {
                string[] _event = p1events[i].Split(' ');
                int year, month, day;
                Int32.TryParse(_event[0].Substring(0, 4), out year);
                Int32.TryParse(_event[0].Substring(5, 2), out month);
                Int32.TryParse(_event[0].Substring(8, 2), out day);
                person1.dayDates.Add(new Date(year, month, day));
            }
            /*
            person1.dayDates.Add(new Date(0));
            person2.dayDates.Add(new Date(0));

            person1.dayDates[0].events.Add(new Event(800, 1000));
            person1.dayDates[0].events.Add(new Event(1400,1600));
            person1.dayDates[0].events.Add(new Event(1800, 1900));

            person2.dayDates[0].events.Add(new Event(1100, 1200));
            person2.dayDates[0].events.Add(new Event(1500, 1600));
            person2.dayDates[0].events.Add(new Event(1900, 2100));

            person1.dayDates.Add(new Date(1));
            person2.dayDates.Add(new Date(1));

            person1.dayDates[1].events.Add(new Event(800, 1000));
            person1.dayDates[1].events.Add(new Event(1400, 1600));

            person2.dayDates[1].events.Add(new Event(1100, 1200));
            person2.dayDates[1].events.Add(new Event(1300, 1400));
            person2.dayDates[1].events.Add(new Event(1900, 2100));
            person2.dayDates[1].events.Add(new Event(2200, 2400));

            Person FreeTimeGuy = getFreeTime(person1, person2);*/
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

            freeTime = invertTime(freeTime);

            return freeTime;
        }

        public static Person invertTime(Person busyTime)
        {
            Person freeTime = new Person();
            for (int i = 0; i < busyTime.dayDates.Count; i++)
            {
                freeTime.dayDates.Add(new Date(busyTime.dayDates[i]._year, busyTime.dayDates[i]._month, busyTime.dayDates[i]._day));
                for (int j = 0; j < busyTime.dayDates[i].events.Count + 1; j++)
                {
                    if (j == 0)
                        if (busyTime.dayDates[i].events[j].startTime != 0)
                        {
                            freeTime.dayDates[i].events.Add(new Event(0, busyTime.dayDates[i].events[j].startTime));
                            continue;
                        }
                    if (j == busyTime.dayDates[i].events.Count)
                        if (busyTime.dayDates[i].events[j - 1].endTime != 2400)
                        {
                            freeTime.dayDates[i].events.Add(new Event(busyTime.dayDates[i].events[j-1].endTime, 2400));
                            continue;
                        }
                        else
                            continue;

                    freeTime.dayDates[i].events.Add(new Event(busyTime.dayDates[i].events[j-1].endTime, busyTime.dayDates[i].events[j].startTime));
                }
            }
            return freeTime;
        }
        public static Date getBusyTimes(Date person1, Date person2)
        {
            Date busyTime = new Date(person1._year,person1._month,person1._day);
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
        public Date(int year,int month,int day)
        {
            _day = day;
            _year = year;
            _month = month;

        }
        public int _year;
        public int _month;
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
