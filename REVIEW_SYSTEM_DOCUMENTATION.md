# Review Controller Implementation

## Overview
Implemented a complete Review system for Managers to review ideas, view voting/comment data, change idea status, and submit reviews with approval/rejection decisions.

## Key Features

### 1. **View All Ideas with Full Details**
- Managers can view all ideas with their:
  - Upvotes and downvotes count
  - All comments with commenter details
  - All reviews submitted on the idea
- Ideas are ordered by most recent submission

### 2. **Filter Ideas by Status**
- Managers can view ideas filtered by status:
  - **Draft** - Ideas in initial draft state
  - **UnderReview** - Ideas being reviewed
  - **Approved** - Ideas that have been approved

### 3. **Change Idea Status**
- Managers can change idea status through the review process
- Valid status transitions: Draft ? UnderReview ? Approved

### 4. **Submit Reviews**
- Managers can submit detailed reviews on ideas
- Reviews include:
  - Feedback/comments
  - Decision (Approve or Reject)
  - Auto-timestamped review date
- One review per manager per idea (enforced by business logic)
- Prevents duplicate reviews from the same manager

### 5. **Review Management**
- Managers can update their own reviews
- Managers can delete their own reviews
- View all reviews for a specific idea
- View all reviews they have submitted

## Created DTOs

### ReviewRequestDto
```csharp
{
  "feedback": "string",
  "decision": "Approve|Reject"
}
```

### ReviewWithIdeaRequestDto
```csharp
{
  "ideaId": "guid",
  "feedback": "string",
  "decision": "Approve|Reject"
}
```

### ReviewResponseDto
```csharp
{
  "reviewId": "guid",
  "ideaId": "guid",
  "reviewerId": "guid",
  "reviewerName": "string",
  "feedback": "string",
  "decision": "Approve|Reject",
  "reviewDate": "datetime"
}
```

### IdeaWithDetailsResponseDto
```csharp
{
  "ideaId": "guid",
  "title": "string",
  "description": "string",
  "categoryId": "guid",
  "categoryName": "string",
  "submittedByUserId": "guid",
  "submittedByUserName": "string",
  "submittedDate": "datetime",
  "status": "Draft|UnderReview|Approved",
  "upvotes": "int",
  "downvotes": "int",
  "comments": [ { CommentResponseDto array } ],
  "reviews": [ { ReviewResponseDto array } ]
}
```

### ChangeIdeaStatusRequestDto
```csharp
{
  "status": "Draft|UnderReview|Approved"
}
```

## API Endpoints

### View Ideas

#### Get All Ideas with Full Details
```
GET /api/review/ideas
Authorization: Bearer {token} (Manager role required)
```

**Response:**
```json
[
  {
    "ideaId": "guid",
    "title": "string",
    "description": "string",
    "categoryId": "guid",
    "categoryName": "string",
    "submittedByUserId": "guid",
    "submittedByUserName": "string",
    "submittedDate": "2024-01-15T10:30:00Z",
    "status": "UnderReview",
    "upvotes": 5,
    "downvotes": 2,
    "comments": [
      {
        "commentId": "guid",
        "userId": "guid",
        "userName": "John Doe",
        "text": "Great idea!",
        "createdDate": "2024-01-15T11:00:00Z"
      }
    ],
    "reviews": [
      {
        "reviewId": "guid",
        "ideaId": "guid",
        "reviewerId": "guid",
        "reviewerName": "Manager Name",
        "feedback": "Strong concept with clear benefits.",
        "decision": "Approve",
        "reviewDate": "2024-01-15T14:00:00Z"
      }
    ]
  }
]
```

#### Get Ideas by Status
```
GET /api/review/ideas/status/{status}
Authorization: Bearer {token} (Manager role required)

{status} values: Draft, UnderReview, Approved
```

#### Get Single Idea with All Details
```
GET /api/review/ideas/{ideaId}
Authorization: Bearer {token} (Manager role required)
```

### Manage Idea Status

#### Change Idea Status
```
PUT /api/review/ideas/{ideaId}/status
Authorization: Bearer {token} (Manager role required)
Content-Type: application/json

{
  "status": "UnderReview"
}
```

**Response:**
```json
{
  "message": "Idea status changed from Draft to UnderReview"
}
```

### Review Operations

#### Submit a Review on an Idea
```
POST /api/review/submit
Authorization: Bearer {token} (Manager role required)
Content-Type: application/json

{
  "ideaId": "550e8400-e29b-41d4-a716-446655440000",
  "feedback": "This is a great idea with strong potential. The implementation plan is clear and aligns with our strategic goals.",
  "decision": "Approve"
}
```

**Response on success:**
```json
{
  "reviewId": "guid",
  "ideaId": "guid",
  "reviewerId": "guid",
  "reviewerName": "Manager Name",
  "feedback": "This is a great idea with strong potential...",
  "decision": "Approve",
  "reviewDate": "2024-01-15T14:00:00Z"
}
```

**Response on duplicate review:**
```json
{
  "message": "You have already submitted a review for this idea"
}
```

#### Get Review by ID
```
GET /api/review/{reviewId}
```

#### Get All Reviews for an Idea
```
GET /api/review/idea/{ideaId}
```

Returns reviews ordered by most recent first.

#### Get All My Reviews (Current Manager)
```
GET /api/review/manager/my-reviews
Authorization: Bearer {token} (Manager role required)
```

#### Update a Review (Own Only)
```
PUT /api/review/{reviewId}
Authorization: Bearer {token} (Manager role required)
Content-Type: application/json

{
  "feedback": "Updated feedback - After further review, this idea has strong merit.",
  "decision": "Approve"
}
```

#### Delete a Review (Own Only)
```
DELETE /api/review/{reviewId}
Authorization: Bearer {token} (Manager role required)
```

## Business Logic

### When Viewing Ideas:
1. Manager can see all ideas in the system
2. Each idea includes:
   - Basic info (title, description, category, submitter)
   - Vote counts (upvotes and downvotes)
   - All comments with commenter information
   - All reviews submitted on the idea
3. Ideas are sorted by submission date (newest first)

### When Changing Status:
1. Manager provides new status
2. System validates status value
3. Status is updated immediately
4. All managers can see the updated status

### When Submitting Review:
1. Manager provides feedback and decision (Approve/Reject)
2. System checks if manager has already reviewed this idea
3. If already reviewed: return error
4. If new: create review with current timestamp
5. Review is immediately visible to all users

### When Updating Review:
1. Only the manager who submitted the review can update it
2. Feedback and decision can be changed
3. Review date is updated to current time
4. All other users can see the updated review

### When Deleting Review:
1. Only the manager who submitted the review can delete it
2. Review is removed from the system
3. Manager can submit a new review if needed

## Authorization & Security

- All review viewing endpoints require "Manager" role
- All review creation/update/deletion endpoints require "Manager" role
- Users can only update/delete their own reviews
- Review reading endpoints are public (AllowAnonymous) for transparency
- Idea status changes require "Manager" role

## Database Constraints

- Reviews are linked to Ideas with Cascade delete behavior
- Reviews are linked to Reviewers (Users) with Restrict delete behavior
- Indexes on: IdeaId, ReviewerId, ReviewDate, Decision
- All review dates are timestamped in UTC

## Error Handling

- User validation (ensuring user exists in token)
- Idea validation (ensuring idea exists)
- Status validation (ensuring valid status values)
- Decision validation (ensuring Approve or Reject)
- Ownership validation (only own review management)
- Duplicate review prevention (one review per manager per idea)
- Comprehensive error messages for all scenarios

## Example Workflow

1. **Employee submits idea** ? Idea created with "Draft" status
2. **Employee votes and comments** ? Upvotes/downvotes with downvote comments
3. **Manager changes status** ? Idea moved to "UnderReview"
4. **Other employees comment** ? More feedback added
5. **Manager submits review** ? Review with Approve/Reject decision
6. **Status updated** ? Idea moved to "Approved"
7. **All users see results** ? Complete idea with all details visible
