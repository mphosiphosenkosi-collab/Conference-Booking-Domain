# Authorization Design Notes — Conference Booking API

## Why Authorization Should Not Live in Controllers

Authorization should not be implemented directly inside controllers because controllers must remain thin and focused only on handling HTTP requests and responses. Embedding role and permission checks in controller logic leads to duplicated rules, harder maintenance, and increased risk of inconsistency. Centralized authorization using ASP.NET Core’s authorization framework ensures consistent enforcement and cleaner separation of concerns.

## Why Roles Belong in Tokens

Roles are included in JWT tokens so that authorization decisions can be made without repeated database lookups. The token carries verified identity and role claims, allowing the API to validate permissions efficiently on each request. This improves performance and supports stateless authentication, which is essential for scalable APIs.

## How This Design Prepares the System for Future Growth

This design supports future expansion by:

- Enabling database-backed ownership rules (booking tied to user ID)
- Supporting fine-grained permission checks without

 I really ApoloGize For The f***** up i did last time i didnt sleep just trying to understand.
