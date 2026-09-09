-- Database schema for the Cybersecurity Awareness Chatbot (Part 3 / POE).
-- Author: Mitchel
-- Run this once in MySQL to create the database and table used by the task assistant.

CREATE DATABASE IF NOT EXISTS cybersecurity_bot;
USE cybersecurity_bot;

CREATE TABLE IF NOT EXISTS tasks (
    id            INT AUTO_INCREMENT PRIMARY KEY,
    title         VARCHAR(255) NOT NULL,
    description   TEXT NULL,
    reminder_date DATETIME NULL,
    is_completed  TINYINT(1) NOT NULL DEFAULT 0,
    created_at    DATETIME NOT NULL
);

-- Optional sample data:
-- INSERT INTO tasks (title, description, reminder_date, is_completed, created_at)
-- VALUES ('Enable two-factor authentication', 'Turn on 2FA for email and banking', NULL, 0, NOW());
