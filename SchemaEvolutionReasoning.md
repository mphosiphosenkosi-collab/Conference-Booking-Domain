# Schema Evolution — Professional Reasoning

**Author:** Siphosenkosi  
**Project:** Conference Room Booking System  
**Date:** February 2026

---

## 1. Why is removing a column more dangerous than adding one?

| Aspect | Adding a Column | Removing a Column |
|--------|-----------------|-------------------|
| **Data** | ✅ Data preserved | ❌ Data permanently lost |
| **Code Impact** | ✅ Backward compatible | ❌ Existing code breaks |
| **Dependencies** | ✅ No effect on other tables | ❌ Views, SPs, indexes may fail |
| **Rollback** | ✅ Simple: just drop column | ❌ Complex: need backup restore |
| **Risk Level** | 🟢 Low | 🔴 High |

**Simple Truth:** Adding is safe because it only adds space. Removing deletes forever.

---

## 2. Why are migrations preferred over manual SQL changes?

| Reason | Migration | Manual SQL |
|--------|-----------|------------|
| **Version Control** | ✅ Tracked in Git | ❌ No history |
| **Repeatability** | ✅ Same on dev/test/prod | ❌ Different every time |
| **Team Collaboration** | ✅ Merge conflicts visible | ❌ Silent overwrites |
| **Rollback** | ✅ `migration down` works | ❌ Must remember exact SQL |
| **CI/CD Integration** | ✅ Runs automatically | ❌ Manual step = forgotten step |

**In Our Project:**  
Every schema change is a migration file in `/Migrations` — versioned, tested, and repeatable.

---

## 3. What could go wrong if two developers modify the schema without migrations?

**Real-World Disaster Scenario:**

```mermaid
Developer A                     Developer B
    |                               |
    | ALTER TABLE Rooms             | ALTER TABLE Bookings
    | ADD COLUMN Location           | ADD COLUMN UserId
    | (runs directly on dev DB)     | (runs directly on dev DB)
    |                               |
    | App works fine                | App works fine
    |                               |
    | Commit code                    | Commit code
    | (no migration file)            | (no migration file)
    |                               |
    └──────────► MERGE ◄────────────┘
                    |
                    ▼
            ❌ PRODUCTION DEPLOY FAILS ❌
            • No migration files exist
            • DB schema doesn't match code
            • No way to recreate changes
            • Rollback? Which SQL to undo?
Problems Without Migrations:

Merge Conflicts — Can't merge database changes like code

Environment Drift — Dev DB ≠ Test DB ≠ Prod DB

Lost Work — One developer's changes overwrite another's

No Audit Trail — Who changed what? When? Why?

Deployment Nightmares — Production schema unknown

4. Which of your schema changes would be risky in production, and why?
🚨 Most Risky: Adding NOT NULL columns without defaults
sql
-- ❌ DANGEROUS: This would crash production!
ALTER TABLE ConferenceRooms 
ADD COLUMN Location TEXT NOT NULL;
-- All existing rows have NULL → VIOLATION!
Why It's Dangerous:

Existing records would fail validation

Application would crash reading old rooms

Requires complex data migration first

✅ Our Safer Approach
Change	How We Implemented	Why It's Safe
Location	string location { get; set; } with default "Unknown"	Old rooms get default value
IsActive	bool IsActive { get; set; } = true	New rooms active by default
CreatedAt	DateTime CreatedAt { get; set; } = DateTime.UtcNow	Auto-set on creation
CancelledAt	DateTime? CancelledAt { get; set; }	Nullable = no forced value
🛡️ Our Risk Mitigation Strategy
Default Values — Every new column has a sensible default

Nullable Fields — Used when default doesn't make sense

Backward Compatibility — Old code still works with new schema

Seed Data — Test data proves changes work

Migration Review — Check generated SQL before applying

📊 Summary Table
Question	One-Line Answer
Remove vs Add?	Removing destroys data; adding just expands
Why migrations?	Version control + repeatability + team safety
No migrations risk?	Environment drift, lost changes, deployment failure
Our riskiest change?	Adding NOT NULL without defaults — we used defaults instead