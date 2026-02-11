# Review Controller - Example API Calls

## Prerequisites
- Replace `{token}` with actual JWT token (Manager role)
- Replace `{ideaId}` with actual idea GUID
- Replace `{reviewId}` with actual review GUID
- API Base URL: `http://localhost:5062`

---

## 1. View All Ideas with Votes and Comments

### Request
```http
GET http://localhost:5062/api/review/ideas
Authorization: Bearer {token}
Accept: application/json
```

### Response (200 OK)
```json
[
  {
    "ideaId": "550e8400-e29b-41d4-a716-446655440000",
    "title": "Implement Real-time Notifications",
    "description": "Add WebSocket-based notifications for instant user updates...",
    "categoryId": "550e8400-e29b-41d4-a716-446655440001",
    "categoryName": "Technology",
    "submittedByUserId": "550e8400-e29b-41d4-a716-446655440002",
    "submittedByUserName": "John Doe",
    "submittedDate": "2024-01-15T10:30:00Z",
    "status": "Draft",
    "upvotes": 3,
    "downvotes": 1,
    "comments": [
      {
        "commentId": "550e8400-e29b-41d4-a716-446655440010",
        "userId": "550e8400-e29b-41d4-a716-446655440003",
        "userName": "Alice Smith",
        "text": "Great idea! This would improve user engagement.",
        "createdDate": "2024-01-15T11:00:00Z"
      },
      {
        "commentId": "550e8400-e29b-41d4-a716-446655440011",
        "userId": "550e8400-e29b-41d4-a716-446655440004",
        "userName": "Bob Johnson",
        "text": "Needs more technical implementation details",
        "createdDate": "2024-01-15T11:30:00Z"
      }
    ],
    "reviews": []
  }
]
```

---

## 2. View Ideas by Status

### Request
```http
GET http://localhost:5062/api/review/ideas/status/UnderReview
Authorization: Bearer {token}
Accept: application/json
```

### Response (200 OK)
Returns ideas filtered by "UnderReview" status with same structure as above.

---

## 3. Get Single Idea with All Details

### Request
```http
GET http://localhost:5062/api/review/ideas/{ideaId}
Authorization: Bearer {token}
Accept: application/json
```

### Response (200 OK)
```json
{
  "ideaId": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Implement Real-time Notifications",
  "description": "Add WebSocket-based notifications...",
  "categoryId": "550e8400-e29b-41d4-a716-446655440001",
  "categoryName": "Technology",
  "submittedByUserId": "550e8400-e29b-41d4-a716-446655440002",
  "submittedByUserName": "John Doe",
  "submittedDate": "2024-01-15T10:30:00Z",
  "status": "UnderReview",
  "upvotes": 5,
  "downvotes": 2,
  "comments": [...],
  "reviews": [...]
}
```

### Error Response (404 Not Found)
```json
{
  "message": "Idea not found"
}
```

---

## 4. Change Idea Status

### Request
```http
PUT http://localhost:5062/api/review/ideas/{ideaId}/status
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "UnderReview"
}
```

### Response (200 OK)
```json
{
  "message": "Idea status changed from Draft to UnderReview"
}
```

### Error Response - Invalid Status (400 Bad Request)
```json
{
  "message": "Invalid status. Valid values are: Draft, UnderReview, Approved"
}
```

### Error Response - Idea Not Found (404)
```json
{
  "message": "Idea not found"
}
```

---

## 5. Submit Review with Approval

### Request
```http
POST http://localhost:5062/api/review/submit
Authorization: Bearer {token}
Content-Type: application/json

{
  "ideaId": "550e8400-e29b-41d4-a716-446655440000",
  "feedback": "Excellent idea with clear benefits. Implementation timeline is realistic. Strong team commitment evident. Recommend approval.",
  "decision": "Approve"
}
```

### Response (201 Created)
```json
{
  "reviewId": "550e8400-e29b-41d4-a716-446655440050",
  "ideaId": "550e8400-e29b-41d4-a716-446655440000",
  "reviewerId": "550e8400-e29b-41d4-a716-446655440005",
  "reviewerName": "Manager Alice",
  "feedback": "Excellent idea with clear benefits...",
  "decision": "Approve",
  "reviewDate": "2024-01-15T14:00:00Z"
}
```

---

## 6. Submit Review with Rejection

### Request
```http
POST http://localhost:5062/api/review/submit
Authorization: Bearer {token}
Content-Type: application/json

{
  "ideaId": "550e8400-e29b-41d4-a716-446655440000",
  "feedback": "While interesting, this idea requires further development and cost-benefit analysis. Recommend revisiting after Q2 planning.",
  "decision": "Reject"
}
```

### Response (201 Created)
```json
{
  "reviewId": "550e8400-e29b-41d4-a716-446655440051",
  "ideaId": "550e8400-e29b-41d4-a716-446655440000",
  "reviewerId": "550e8400-e29b-41d4-a716-446655440006",
  "reviewerName": "Manager Bob",
  "feedback": "While interesting, this idea requires further development...",
  "decision": "Reject",
  "reviewDate": "2024-01-15T14:15:00Z"
}
```

### Error Response - Duplicate Review (400 Bad Request)
```json
{
  "message": "You have already submitted a review for this idea"
}
```

### Error Response - Missing Idea (404)
```json
{
  "message": "Idea not found"
}
```

---

## 7. Get Review by ID

### Request
```http
GET http://localhost:5062/api/review/{reviewId}
Accept: application/json
```

### Response (200 OK)
```json
{
  "reviewId": "550e8400-e29b-41d4-a716-446655440050",
  "ideaId": "550e8400-e29b-41d4-a716-446655440000",
  "reviewerId": "550e8400-e29b-41d4-a716-446655440005",
  "reviewerName": "Manager Alice",
  "feedback": "Excellent idea with clear benefits...",
  "decision": "Approve",
  "reviewDate": "2024-01-15T14:00:00Z"
}
```

---

## 8. Get All Reviews for Idea

### Request
```http
GET http://localhost:5062/api/review/idea/{ideaId}
Accept: application/json
```

### Response (200 OK)
```json
[
  {
    "reviewId": "550e8400-e29b-41d4-a716-446655440050",
    "ideaId": "550e8400-e29b-41d4-a716-446655440000",
    "reviewerId": "550e8400-e29b-41d4-a716-446655440005",
    "reviewerName": "Manager Alice",
    "feedback": "Excellent idea...",
    "decision": "Approve",
    "reviewDate": "2024-01-15T14:00:00Z"
  },
  {
    "reviewId": "550e8400-e29b-41d4-a716-446655440051",
    "ideaId": "550e8400-e29b-41d4-a716-446655440000",
    "reviewerId": "550e8400-e29b-41d4-a716-446655440006",
    "reviewerName": "Manager Bob",
    "feedback": "While interesting...",
    "decision": "Reject",
    "reviewDate": "2024-01-15T14:15:00Z"
  }
]
```

---

## 9. Get My Reviews (Current Manager)

### Request
```http
GET http://localhost:5062/api/review/manager/my-reviews
Authorization: Bearer {token}
Accept: application/json
```

### Response (200 OK)
```json
[
  {
    "reviewId": "550e8400-e29b-41d4-a716-446655440050",
    "ideaId": "550e8400-e29b-41d4-a716-446655440000",
    "reviewerId": "550e8400-e29b-41d4-a716-446655440005",
    "reviewerName": "Manager Alice",
    "feedback": "Excellent idea with clear benefits...",
    "decision": "Approve",
    "reviewDate": "2024-01-15T14:00:00Z"
  },
  {
    "reviewId": "550e8400-e29b-41d4-a716-446655440052",
    "ideaId": "550e8400-e29b-41d4-a716-446655440001",
    "reviewerId": "550e8400-e29b-41d4-a716-446655440005",
    "reviewerName": "Manager Alice",
    "feedback": "Good idea, needs refinement...",
    "decision": "Reject",
    "reviewDate": "2024-01-15T15:00:00Z"
  }
]
```

---

## 10. Update Own Review

### Request
```http
PUT http://localhost:5062/api/review/{reviewId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "feedback": "Updated assessment: After reviewing additional details, this idea has stronger merit than initially thought. Recommend approval.",
  "decision": "Approve"
}
```

### Response (200 OK)
```json
{
  "message": "Review updated successfully",
  "review": {
    "reviewId": "550e8400-e29b-41d4-a716-446655440050",
    "ideaId": "550e8400-e29b-41d4-a716-446655440000",
    "reviewerId": "550e8400-e29b-41d4-a716-446655440005",
    "reviewerName": "Manager Alice",
    "feedback": "Updated assessment: After reviewing additional details...",
    "decision": "Approve",
    "reviewDate": "2024-01-15T14:30:00Z"
  }
}
```

### Error Response - Not Own Review (403 Forbidden)
```json
{
  "message": "You can only update your own reviews"
}
```

---

## 11. Delete Own Review

### Request
```http
DELETE http://localhost:5062/api/review/{reviewId}
Authorization: Bearer {token}
Content-Type: application/json
```

### Response (200 OK)
```json
{
  "message": "Review deleted successfully"
}
```

### Error Response - Not Own Review (403 Forbidden)
```json
{
  "message": "You can only delete your own reviews"
}
```

---

## Common HTTP Status Codes

| Status | Meaning | Example |
|--------|---------|---------|
| 200 | OK | Successful GET/PUT/DELETE |
| 201 | Created | Review created successfully |
| 400 | Bad Request | Invalid status, missing feedback, duplicate review |
| 403 | Forbidden | Trying to update/delete someone else's review |
| 404 | Not Found | Idea/review not found |
| 401 | Unauthorized | Missing or invalid token |

---

## Notes

- All timestamps are in UTC (Z suffix)
- Manager role required for most write operations
- Public endpoints allow reading reviews and ideas (no auth needed)
- Review dates are auto-updated when modified
- One review per manager per idea is enforced
