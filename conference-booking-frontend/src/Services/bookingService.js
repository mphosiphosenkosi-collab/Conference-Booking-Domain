// ============================================
// bookingService.js — Mock API Service
// ============================================
//
// Simulates:
// • Network delay
// • Random server failures
// • Realistic booking data shape
//
// Used to demonstrate:
// • Async fetching
// • Error handling
// • Loading states
// • Retry logic
//
// ============================================

export function fetchAllBookings() {
  return new Promise((resolve, reject) => {

    // Random delay between 500–2500 ms
    const delay = Math.floor(Math.random() * 2000) + 500;

    setTimeout(() => {

      // 20% simulated failure rate
      const shouldFail = Math.random() < 0.2;

      if (shouldFail) {
        reject(new Error("Server temporarily unavailable"));
        return;
      }

     
      resolve([
        {
          id: 1,
          roomName: "A101",
          customerName: "Finance Team",
          status: "confirmed",
          category: "internal"
        },
        {
          id: 2,
          roomName: "B202",
          customerName: "Client Alpha",
          status: "pending",
          category: "client"
        },
        {
          id: 3,
          roomName: "C303",
          customerName: "HR Meeting",
          status: "confirmed",
          category: "internal"
        },
        {
          id: 4,
          roomName: "D404",
          customerName: "Client Beta",
          status: "pending",
          category: "client"
        }
      ]);

    }, delay);

  });
}
