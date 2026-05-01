-- Reference schema mirrored by `SqliteDatabaseMigrator` (SQLite).
-- Useful if you want to inspect structure outside the application bootstrap.

PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Users (
  Id TEXT PRIMARY KEY NOT NULL,
  Email TEXT NOT NULL UNIQUE COLLATE NOCASE,
  PasswordHash TEXT NOT NULL,
  DisplayName TEXT NOT NULL,
  CreatedAtUtc TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS FeatureFlags (
  Id TEXT PRIMARY KEY NOT NULL,
  OwnerUserId TEXT NOT NULL,
  Key TEXT NOT NULL,
  Description TEXT NOT NULL,
  IsEnabled INTEGER NOT NULL,
  Environment TEXT NOT NULL,
  AiIntegrationHintsJson TEXT NOT NULL,
  CreatedAtUtc TEXT NOT NULL,
  UpdatedAtUtc TEXT NOT NULL,
  UNIQUE (OwnerUserId, Key),
  FOREIGN KEY (OwnerUserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_FeatureFlags_Owner ON FeatureFlags(OwnerUserId);
CREATE INDEX IF NOT EXISTS IX_FeatureFlags_Environment ON FeatureFlags(Environment);
