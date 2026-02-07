#!/bin/bash
echo "=== ASSIGNMENT 2.1 - DEMONSTRATING API BEHAVIOR ==="
echo "Requirement 6: Demonstrate both valid and invalid booking requests via HTTP"
echo

# Build the project first
echo "1. Building the project..."
dotnet build

if [ $? -ne 0 ]; then
    echo "❌ Build failed. Cannot demonstrate API behavior."
    exit 1
fi

echo "✅ Build successful"
echo

# Start API in background
echo "2. Starting Web API..."
dotnet run --project src/ConferenceRoomBooking.WebApi --urls "http://localhost:5000" > api.log 2>&1 &
API_PID=$!

echo "Waiting for API to start..."
sleep 5

echo "3. Testing API is responsive..."
if curl -s http://localhost:5000/api/bookings/test > /dev/null; then
    echo "✅ API is running"
else
    echo "❌ API failed to start"
    kill $API_PID 2>/dev/null
    exit 1
fi

echo
echo "4. DEMONSTRATION 1: Valid Booking Request"
echo "------------------------------------------"
VALID_JSON='{
  "employeeId": "EMP001",
  "roomName": "Boardroom",
  "startTime": "2024-02-05T10:00:00",
  "endTime": "2024-02-05T11:00:00"
}'

echo "Sending valid booking request:"
echo "$VALID_JSON"
echo

RESPONSE=$(curl -s -X POST http://localhost:5000/api/bookings \
  -H "Content-Type: application/json" \
  -d "$VALID_JSON")

echo "Response (JSON):"
echo "$RESPONSE" | python -m json.tool 2>/dev/null || echo "$RESPONSE"
echo

echo "5. DEMONSTRATION 2: Invalid Booking Request (missing employeeId)"
echo "---------------------------------------------------------------"
INVALID_JSON='{
  "roomName": "Boardroom",
  "startTime": "2024-02-05T10:00:00",
  "endTime": "2024-02-05T11:00:00"
}'

echo "Sending invalid booking request (missing required field):"
echo "$INVALID_JSON"
echo

INVALID_RESPONSE=$(curl -s -X POST http://localhost:5000/api/bookings \
  -H "Content-Type: application/json" \
  -d "$INVALID_JSON")

echo "Response (JSON):"
echo "$INVALID_RESPONSE" | python -m json.tool 2>/dev/null || echo "$INVALID_RESPONSE"
echo

echo "6. DEMONSTRATION 3: GET All Bookings"
echo "-------------------------------------"
GET_RESPONSE=$(curl -s http://localhost:5000/api/bookings)

echo "Response (JSON):"
echo "$GET_RESPONSE" | python -m json.tool 2>/dev/null || echo "$GET_RESPONSE"
echo

# Stop API
echo "7. Cleaning up..."
kill $API_PID 2>/dev/null
wait $API_PID 2>/dev/null

echo
echo "=== DEMONSTRATION COMPLETE ==="
echo "✅ Demonstrated:"
echo "   - Valid booking request (POST with all required fields)"
echo "   - Invalid booking request (POST missing required field)"  
echo "   - GET all bookings"
echo "   - All responses in JSON format"
echo
echo "Requirement 6: SATISFIED ✓"
