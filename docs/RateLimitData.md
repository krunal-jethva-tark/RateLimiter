## Rate Limit Data: `RateLimitData`

The `RateLimitData` class encapsulates the essential data used by rate limiting strategies to track and enforce request limits. This class provides a structure to store and manage rate limit information across different strategies, such as fixed window, token bucket, and others.

### Key Features

- **Count**: 
  - **Description**: Keeps track of the number of requests made within a defined time frame.
  - **Type**: `int`
  
- **Tokens Available**: 
  - **Description**: Indicates the current number of tokens available in token-based strategies.
  - **Type**: `int`
  
- **Last Refill Time**: 
  - **Description**: Tracks when the last refill of tokens occurred, which is essential for determining token availability.
  - **Type**: `DateTime`
  
- **Expiration**: 
  - **Description**: Defines how long the rate limit data is valid before it needs to be refreshed.
  - **Type**: `TimeSpan`
  
- **Creation Time**: 
  - **Description**: Records when the rate limit data instance was created.
  - **Type**: `DateTime`


### When to Use

The `RateLimitData` class is particularly useful in the following scenarios:

- **Rate Limiting Implementations**: 
  - Utilize this class to store data related to user requests and manage limits based on your chosen strategy (e.g., fixed window, sliding window, token bucket).
  
- **State Management**: 
  - Helps in tracking request counts, token availability, and expiration to effectively enforce rate limiting policies.

### Prerequisites

- This class is typically used in conjunction with other rate limiting strategy implementations that leverage counting or token-based methods. Ensure that your rate limiting strategy can effectively interact with the data contained in `RateLimitData`.