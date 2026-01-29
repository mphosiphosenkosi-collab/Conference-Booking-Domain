#!/bin/bash
echo "=== Testing Conference Room Booking System ==="
echo "1. Building project..."
dotnet build

echo ""
echo "2. Creating test data file..."
cat > bookings_data.json << 'JSON'
{
  "Rooms": [
    {"Id": 1, "Name": "Executive Boardroom", "Capacity": 20, "Type": 1},
    {"Id": 2, "Name": "Training Center", "Capacity": 50, "Type": 2},
    {"Id": 3, "Name": "Huddle Space", "Capacity": 6, "Type": 0},
    {"Id": 4, "Name": "Video Conference Suite", "Capacity": 12, "Type": 3},
    {"Id": 5, "Name": "Innovation Lab", "Capacity": 15, "Type": 0},
    {"Id": 6, "Name": "Client Meeting Room", "Capacity": 8, "Type": 1}
  ],
  "Bookings": []
}
JSON

echo "✅ Created test data file"
echo ""
echo "3. Running program..."
echo "   Select Option 1 to view rooms"
echo "   Select Option 2 to create a booking"
echo "   Select Option 8 to run A1.3 demos"
echo "   Select Option 9 to exit"
echo ""
dotnet run --project ./src/ConferenceRoomBooking.ConsoleApp
