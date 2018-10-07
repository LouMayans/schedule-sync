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
            int datecount = -1;
            for (int i = 0; i < p1events.Length- 1; i++)
            {
                string[] _event = p1events[i].Split(' ');
                int year, month, day;
                Int32.TryParse(_event[0].Substring(0, 4), out year);
                Int32.TryParse(_event[0].Substring(5, 2), out month);
                Int32.TryParse(_event[0].Substring(8, 2), out day);
                Date temp = new Date(year, month, day);

                if (person1.dayDates.Count == 0)
                {
                    person1.dayDates.Add(temp);
                    ++datecount;
                }
                else
                {
                    if(!(person1.dayDates[datecount]._year == temp._year && person1.dayDates[datecount]._month == temp._month && person1.dayDates[datecount]._day == temp._day))
                    {
                        person1.dayDates.Add(temp);
                        ++datecount;
                    }
                }

                int start, end;
                string startS = _event[0].Substring(11, 2) + _event[0].Substring(14, 2);
                string endS = _event[1].Substring(11, 2) + _event[1].Substring(14, 2);
                Int32.TryParse(startS, out start);
                Int32.TryParse(endS, out end);

                person1.dayDates[datecount].events.Add(new Event(start, end));
            }
            datecount = -1;
            for (int i = 0; i < p2events.Length - 1; i++)
            {
                string[] _event = p2events[i].Split(' ');
                int year, month, day;
                Int32.TryParse(_event[0].Substring(0, 4), out year);
                Int32.TryParse(_event[0].Substring(5, 2), out month);
                Int32.TryParse(_event[0].Substring(8, 2), out day);
                Date temp = new Date(year, month, day);

                if (person2.dayDates.Count == 0)
                {
                    person2.dayDates.Add(temp);
                    ++datecount;
                }
                else
                {
                    if (!(person2.dayDates[datecount]._year == temp._year && person2.dayDates[datecount]._month == temp._month && person2.dayDates[datecount]._day == temp._day))
                    {
                        person2.dayDates.Add(temp);
                        ++datecount;
                    }
                }

                int start, end;
                string startS = _event[0].Substring(11, 2) + _event[0].Substring(14, 2);
                string endS = _event[1].Substring(11, 2) + _event[1].Substring(14, 2);
                Int32.TryParse(startS, out start);
                Int32.TryParse(endS, out end);

                person2.dayDates[datecount].events.Add(new Event(start, end));
            }

            

            Person FreeTimeGuy = getFreeTime(person1, person2);
        }

        public static Person getFreeTime(Person person1, Person person2)
        {
            Person freeTime = new Person();

            Person current;
            Person after;
            if (person1.dayDates.Count > person2.dayDates.Count)
            {
                current = person1;
                after = person2;
            }
            else
            {
                current = person2;
                after = person1;
            }

            for (int i = 0; i < current.dayDates.Count; i++)
            {

                bool checkFound = false;
                foreach (Date date in after.dayDates)
                {
                    if (current.dayDates[i]._year == date._year && current.dayDates[i]._month == date._month && current.dayDates[i]._day == date._day)
                    {
                        freeTime.dayDates.Add(getBusyTimes(current.dayDates[i], date));
                        checkFound = true;
                        break;
                    }
                }

                if(!checkFound)
                {
                    freeTime.dayDates.Add(getBusyTimes(current.dayDates[i], new Date(current.dayDates[i]._year, current.dayDates[i]._month, current.dayDates[i]._day)));
                }

                

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
                    if(j != 0)
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
                else if (person2.events.Count == person2Count)
                {
                    busyTime.events.Add(new Event(person1.events[person1Count].startTime, person1.events[person1Count].endTime));
                    ++person1Count;
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
                            ++person1Count;
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
                            ++person2Count;
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
