# Fitness Tracker Web Application

A comprehensive C# ASP.NET Core MVC web application for tracking fitness activities and monitoring progress towards daily calorie goals.

## Features

### User Management

- **User Registration**: Create accounts with username/password validation
  - Username: Only letters and numbers allowed
  - Password: Exactly 12 characters with at least one uppercase and one lowercase letter
- **User Authentication**: Secure login with failed attempt tracking and account locking
- **Session Management**: Persistent user sessions across the application

### Fitness Activity Tracking

- **Six Activity Types Supported**:
  1. **Walking**: Steps, Distance (km), Time (minutes)
  2. **Swimming**: Laps, Time (minutes), Average Heart Rate
  3. **Running**: Distance (km), Time (minutes), Average Heart Rate
  4. **Cycling**: Distance (km), Time (minutes), Average Heart Rate
  5. **Weight Lifting**: Sets, Weight (kg), Reps
  6. **Yoga**: Poses, Time (minutes), Intensity (1-10)

### Calorie Calculation

- **Research-Based Formulas**: Automatic calorie calculation using all three metrics per activity
- **Activity-Specific Calculations**: Different formulas for each activity type
- **Real-Time Updates**: Calories recalculated when activities are edited

### Goal Setting & Progress Monitoring

- **Personal Goals**: Set daily calorie burn targets
- **Progress Tracking**: Visual progress bars and achievement indicators
- **Goal Achievement**: Notifications when daily goals are met
- **Progress Reports**: Calculate total calories burned and remaining goals

### User Interface

- **Responsive Design**: Bootstrap-based UI that works on all devices
- **Intuitive Navigation**: Clear, labeled controls with icons
- **Dashboard**: Comprehensive overview of daily progress and recent activities
- **Activity Management**: Full CRUD operations for fitness activities
- **Error Handling**: Appropriate error messages and user guidance

## Technical Requirements Met

### Functional Requirements ✅

- [x] User registration and login with validation
- [x] Failed login attempt handling with account locking
- [x] Three predefined metrics for each of six activities
- [x] Calorie calculation based on activity metrics
- [x] Total calorie calculation and reporting
- [x] Personal fitness goal setting connected to user accounts
- [x] Goal achievement reporting

### Non-Functional Requirements ✅

- [x] GUI with consistent layout and intuitive controls
- [x] Username and password validation as specified
- [x] Comprehensive error messages and user guidance
- [x] Efficient data handling and user experience

### Technical Implementation ✅

- [x] C# ASP.NET Core MVC architecture
- [x] Entity Framework Core with SQLite database
- [x] Object-Oriented Programming principles
- [x] Proper encapsulation in model classes
- [x] Control structures (loops, conditionals) in business logic
- [x] Comprehensive code comments and documentation

## Architecture

### Models

- **User**: User account management with validation
- **ActivityRecord**: Individual fitness activity tracking
- **ActivityType**: Enumeration of supported activities
- **CalorieCalculator**: Static service for calorie calculations

### Controllers

- **AccountController**: User authentication and registration
- **HomeController**: Dashboard and goal management
- **ActivitiesController**: Activity CRUD operations

### Database

- **SQLite Database**: Lightweight, cross-platform data storage
- **Entity Framework**: Code-first approach with migrations
- **Relationships**: User has many ActivityRecords (one-to-many)

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- Visual Studio Code (recommended)

### Running the Application

1. **Clone/Open the project**
2. **Restore dependencies**:

   ```bash
   dotnet restore
   ```

3. **Build the application**:

   ```bash
   dotnet build
   ```

4. **Run the application**:

   ```bash
   dotnet run --project FitnessTracker/FitnessTracker.csproj --urls=http://localhost:5000
   ```

5. **Access the application**:
   Open your browser and navigate to http://localhost:5000

### Default Test Account

- Username: `testuser123`
- Password: `TestPass123A`
- Goal: 300 calories

### First Time Setup

1. Navigate to http://localhost:5000
2. Register a new account or use the test account
3. Set your daily calorie goal
4. Start logging activities to track your progress

## Activity Calorie Formulas

Each activity uses research-backed formulas incorporating all three metrics:

### Walking

- Formula: METs × weight × time × step efficiency
- Considers walking speed and step count for accuracy

### Swimming

- Formula: Base METs × heart rate intensity × lap intensity × weight × time
- Accounts for workout intensity and swimming efficiency

### Running

- Formula: Speed-based METs × heart rate multiplier × weight × time
- Adjusts for running pace and cardiovascular intensity

### Cycling

- Formula: Speed-based METs × heart rate multiplier × weight × time
- Considers cycling speed and effort level

### Weight Lifting

- Formula: Base METs × intensity × volume multiplier × weight × estimated time
- Factors in weight lifted, sets, reps, and training volume

### Yoga

- Formula: Intensity-based METs × pose complexity × weight × time
- Adjusts for yoga style and pose difficulty

## Database Schema

### Users Table

- Id (Primary Key)
- Username (Unique, alphanumeric only)
- Password (Hashed, 12 characters)
- CalorieGoal (Daily target)
- FailedLoginAttempts (Security tracking)
- IsLocked (Account security)
- CreatedAt (Registration timestamp)

### ActivityRecords Table

- Id (Primary Key)
- UserId (Foreign Key)
- ActivityType (Enum)
- Metric1, Metric2, Metric3 (Activity measurements)
- CaloriesBurned (Calculated value)
- RecordedAt (Activity timestamp)

## Security Features

- **Password Hashing**: SHA256 with salt (production should use bcrypt)
- **Session Management**: Secure session handling
- **Failed Login Protection**: Account locking after 5 failed attempts
- **Input Validation**: Server-side validation for all user inputs
- **CSRF Protection**: Anti-forgery tokens on forms

## Future Enhancements

- User profile management
- Activity history charts and analytics
- Social features and challenges
- Mobile app companion
- Integration with fitness devices
- Advanced reporting and insights

## License

This project is created for educational purposes as part of a fitness tracking software assignment.
