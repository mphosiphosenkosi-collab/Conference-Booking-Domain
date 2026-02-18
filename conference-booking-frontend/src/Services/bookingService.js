export function fetchAllBookings() {
  return new Promise((resolve, reject) => {
    // random delay between 500 and 2500 ms
    const delay = Math.floor(Math.random() * 2000) + 500;

    setTimeout(() => {
      // 20% chance to fail
      const shouldFail = Math.random() < 0.2;

      if (shouldFail) {
        reject(new Error("Server temporarily unavailable"));
        return;
      }

      // fake booking data
      resolve([
        { id: 1, room: "A101", category: "Internal" },
        { id: 2, room: "B202", category: "Client" },
        { id: 3, room: "C303", category: "Internal" }
      ]);
    }, delay);
  });
}
