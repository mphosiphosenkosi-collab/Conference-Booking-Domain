#!/bin/bash
echo "=== FINAL STRUCTURE VERIFICATION ==="
echo

echo "1. SOLUTION FILE INTEGRITY:"
echo "---------------------------"
if dotnet sln list | grep -q "ConferenceRoomBooking.API"; then
    echo "❌ ConferenceRoomBooking.API still in solution"
    echo "   Run: dotnet sln remove src/ConferenceRoomBooking.API"
else
    echo "✅ ConferenceRoomBooking.API NOT in solution"
fi

if dotnet sln list | grep -q "ConferenceRoomBooking.WebApi"; then
    echo "✅ ConferenceRoomBooking.WebApi in solution"
else
    echo "❌ ConferenceRoomBooking.WebApi missing from solution"
    echo "   Run: dotnet sln add src/ConferenceRoomBooking.WebApi"
fi

echo -e "\n2. PROJECT COUNT:"
echo "----------------"
EXPECTED=5
ACTUAL=$(dotnet sln list | grep -c "\.csproj$")
if [ $ACTUAL -eq $EXPECTED ]; then
    echo "✅ Correct: $ACTUAL projects in solution"
    dotnet sln list | grep "\.csproj$"
else
    echo "❌ Incorrect: Found $ACTUAL projects, expected $EXPECTED"
fi

echo -e "\n3. BUILD STATUS:"
echo "----------------"
if dotnet build --verbosity quiet; then
    echo "✅ All projects build successfully"
    echo -e "\n��� STRUCTURE IS CORRECT AND READY FOR ASSIGNMENT 2.1!"
    echo
    echo "WebApi project location: src/ConferenceRoomBooking.WebApi/"
    echo "Run with: dotnet run --project src/ConferenceRoomBooking.WebApi"
    echo "Test with: curl http://localhost:5000/api/bookings"
else
    echo "❌ Build failed"
    exit 1
fi
