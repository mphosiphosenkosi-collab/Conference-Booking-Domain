using System;

using System.Collections.Generic;
using ConferenceRoomBooking.Domain.Enums;

namespace ConferenceRoomBooking.Domain.Entities
{
    public class ConferenceRoom
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string RoomType { get; set; }
        public ConferenceRoom(string name, int capacity, string roomType)
        {
            Name = name;
            Capacity = capacity;
            RoomType = roomType;
        }
    }

 }