using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Examen.Infrastructure.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time notification delivery
    /// Handles in-app alert notifications via WebSocket
    /// </summary>
    [Authorize]
    public class NotificationHub : Hub
    {
        private const int MaxGroupNameLength = 50;
        private const string ValidGroupNamePattern = @"^[a-zA-Z0-9_-]+$";

        /// <summary>
        /// Join a user-specific group for personalized notifications
        /// User can only join their own group
        /// </summary>
        public async Task JoinUserGroup(string userId)
        {
            try
            {
                // Validate that the user is joining their own group
                var currentUserId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(currentUserId) || currentUserId != userId)
                {
                    throw new HubException("You can only join your own user group");
                }

                if (string.IsNullOrEmpty(userId) || userId.Length > MaxGroupNameLength)
                {
                    throw new HubException("Invalid user ID format");
                }

                var groupName = $"user_{userId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
            catch (HubException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HubException($"Failed to join user group: {ex.Message}");
            }
        }

        /// <summary>
        /// Join a role-based group for notifications targeted at roles
        /// User must have the role in their claims to join
        /// </summary>
        public async Task JoinRoleGroup(string role)
        {
            try
            {
                if (string.IsNullOrEmpty(role))
                {
                    throw new HubException("Role cannot be empty");
                }

                if (role.Length > MaxGroupNameLength)
                {
                    throw new HubException("Role name is too long");
                }

                // Validate role format - only alphanumeric, dash, underscore
                if (!System.Text.RegularExpressions.Regex.IsMatch(role, ValidGroupNamePattern))
                {
                    throw new HubException("Role contains invalid characters");
                }

                // Verify user has this role in their claims
                var userRoles = Context.User?
                    .FindAll(ClaimTypes.Role)
                    ?.Select(c => c.Value)
                    .ToList() ?? new();

                if (!userRoles.Contains(role))
                {
                    throw new HubException($"You don't have the '{role}' role");
                }

                var groupName = $"role_{role}";
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
            catch (HubException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HubException($"Failed to join role group: {ex.Message}");
            }
        }

        /// <summary>
        /// Called when a client disconnects from the hub
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                // You could log disconnection errors here
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}