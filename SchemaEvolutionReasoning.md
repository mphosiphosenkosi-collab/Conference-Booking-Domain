# Schema Evolution Professional Reasoning

## 1. Why is removing a column more dangerous than adding one?

Removing a column is more dangerous because:

- **Data Loss:** All data in that column is permanently deleted
- **Application Breakage:** Any part of the application referencing that column will fail
- **Dependencies:** Other tables, views, stored procedures, or application code might depend on it
- **Rollback Complexity:** Adding a column back requires restoring data from backups

Adding a column is safer because:

- It's backward compatible (existing code continues to work)
- No data is lost
- Can be rolled back easily by removing the column

## 2. Why are migrations preferred over manual SQL changes?

Migrations are preferred because:

- **Version Control:** Changes are tracked in code, not just in database
- **Repeatability:** Same migration can be applied to dev, test, and prod
- **Collaboration:** Multiple developers can work on schema changes
- **Rollback:** Migrations support both "up" and "down" operations
- **Consistency:** Ensures all environments have identical schema
- **Automation:** Can be integrated into CI/CD pipelines

## 3. What could go wrong if two developers modify the schema without migrations?

Without migrations:

- **Merge Conflicts:** Direct database changes can't be merged like code
- **Environment Drift:** Different environments get out of sync
- **Lost Changes:** One developer's changes might overwrite another's
- **No Audit Trail:** No record of who changed what and when
- **Deployment Issues:** Production schema might differ from development
- **Data Loss:** Manual SQL might accidentally delete data

## 4. Which of your schema changes would be risky in production, and why?

### *Most Risky Change: Adding NOT NULL columns without defaults**

If we added `Location` to `ConferenceRoom` as NOT NULL without a default:

- Existing records would fail validation
- Application would crash when reading existing rooms
- Would require data migration before schema change

**Our Safer Approach:**

- Made `Location` nullable OR provided a default value ("Unknown")
- Made `IsActive` with default value (true)
- Made `CreatedAt` with default SQL function
- Made `CancelledAt` nullable

**Risk Mitigation:**

1. All new columns have sensible defaults
2. Nullable columns where appropriate
3. Backward compatible changes
4. Data seeded for new requirements
