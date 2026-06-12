# 🐾 4Paws API

A comprehensive pet care platform that connects pet owners with caregivers. This REST API enables users to find trusted pet sitters, manage agreements, submit reviews, and handle pet-related services efficiently.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Authentication](#authentication)
- [Database Schema](#database-schema)
- [Testing](#testing)
- [Environment Setup](#environment-setup)
- [Contributing](#contributing)

---

## 🎯 Overview

4Paws is a pet care marketplace that simplifies the process of finding and booking pet care services. It provides a secure platform where:

- **Pet Owners** can post listings requesting pet care services and review caregivers
- **Caregivers** can offer their services and build their reputation
- **Administrators** can manage users and platform integrity

---

## ✨ Features

### 🔐 Authentication & Authorization
- User registration with email verification
- Secure JWT-based authentication
- Role-based access control (Owner, CareGiver, Admin)
- Password reset functionality with verification codes
- Account suspension for banned users

### 📋 Listings Management
- Create and manage pet care listings
- Two listing types:
  - **Owner Needs CareGiver**: Pet owners request specific pet care
  - **CareGiver Offers Service**: Caregivers advertise their services
- Listing status tracking (Open, Closed, Expired)
- In-memory caching for improved performance (5-minute TTL)

### 📝 Agreements & Contracts
- Create formal agreements between owners and caregivers
- Agreement status tracking (Pending, Active, Completed, Cancelled)
- Automatic agreement expiration handling

### ⭐ Reviews & Ratings
- Submit reviews after service completion
- 5-point rating system (Poor, Fair, Average, Good, Excellent)
- Prevent duplicate and self-reviews
- Automatic rating calculation for users and pets

### 👥 User Profiles
- Separate Owner and CareGiver profiles
- Profile rating and reputation management
- Avatar/image upload support
- Bio and service information for caregivers

### 🐕 Pet Management
- Create and manage pet profiles
- Pet types and breeds tracking
- Pet-specific reviews and ratings

### 📧 Email Notifications
- Email verification on registration
- Password reset codes via email
- Service notifications and updates

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | ASP.NET Core 8.0 |
| **Database** | SQL Server with Entity Framework Core 8.0 |
| **Authentication** | JWT Bearer Tokens |
| **Password Hashing** | BCrypt.Net-Next |
| **Validation** | FluentValidation 12.1.1 |
| **Mapping** | AutoMapper 12.0.1 |
| **File Storage** | Cloudinary |
| **Testing** | xUnit, Moq, FluentAssertions |

---

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK or later
- SQL Server (local or cloud)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**