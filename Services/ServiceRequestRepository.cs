using System;
using System.Collections.Generic;
using MunicipalReporter.Models;

namespace MunicipalReporter.Services
{
    public class ServiceRequestRepository
    {
        private readonly Dictionary<string, List<string>> statusTransitions =
            new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Submitted", new List<string> { "Under Review", "Approved", "Rejected" } },
                { "Under Review", new List<string> { "Approved", "Rejected" } },
                { "Approved", new List<string> { "In Progress" } },
                { "In Progress", new List<string> { "Resolved" } },
                { "Rejected", new List<string>() },
                { "Resolved", new List<string>() }
            };

        private Node root;
        private int count;

        public int Count
        {
            get { return count; }
        }

        public void Add(MunicipalIssue issue)
        {
            if (issue == null)
            {
                throw new ArgumentNullException("issue");
            }

            if (GetByReference(issue.ReferenceNumber) != null)
            {
                throw new ArgumentException("A service request with this reference number already exists.", "issue");
            }

            root = Insert(root, issue);
            count++;
        }

        public MunicipalIssue GetByReference(string referenceNumber)
        {
            if (string.IsNullOrWhiteSpace(referenceNumber))
            {
                return null;
            }

            Node current = root;
            while (current != null)
            {
                int comparison = StringComparer.OrdinalIgnoreCase.Compare(referenceNumber.Trim(), current.Issue.ReferenceNumber);
                if (comparison == 0)
                {
                    return current.Issue;
                }

                current = comparison < 0 ? current.Left : current.Right;
            }

            return null;
        }

        public List<MunicipalIssue> GetAll()
        {
            List<MunicipalIssue> result = new List<MunicipalIssue>(count);
            AddInOrder(root, result);
            return result;
        }

        public List<string> GetAllowedNextStatuses(string referenceNumber)
        {
            MunicipalIssue issue = GetByReference(referenceNumber);
            if (issue == null)
            {
                return new List<string>();
            }

            List<string> allowedStatuses;
            return statusTransitions.TryGetValue(issue.Status, out allowedStatuses)
                ? new List<string>(allowedStatuses)
                : new List<string>();
        }

        public void ChangeStatus(string referenceNumber, string newStatus)
        {
            MunicipalIssue issue = GetByReference(referenceNumber);
            if (issue == null)
            {
                throw new KeyNotFoundException("No service request was found with reference number " + referenceNumber + ".");
            }

            List<string> allowedStatuses;
            if (!statusTransitions.TryGetValue(issue.Status, out allowedStatuses) ||
                !allowedStatuses.Contains(newStatus))
            {
                throw new InvalidOperationException(
                    "A request cannot move from " + issue.Status + " to " + newStatus + ".");
            }

            issue.UpdateStatus(newStatus);
        }

        public List<string> GetWorkflowPath(string currentStatus)
        {
            if (string.IsNullOrWhiteSpace(currentStatus) || !statusTransitions.ContainsKey(currentStatus))
            {
                return new List<string>();
            }

            Queue<string> pending = new Queue<string>();
            Dictionary<string, string> previousStatus =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            pending.Enqueue("Submitted");
            visited.Add("Submitted");

            while (pending.Count > 0)
            {
                string status = pending.Dequeue();
                if (string.Equals(status, currentStatus, StringComparison.OrdinalIgnoreCase))
                {
                    List<string> path = new List<string>();
                    string pathStatus = status;
                    while (pathStatus != null)
                    {
                        path.Add(pathStatus);
                        string previous;
                        pathStatus = previousStatus.TryGetValue(pathStatus, out previous) ? previous : null;
                    }

                    path.Reverse();
                    return path;
                }

                List<string> nextStatuses = statusTransitions[status];
                foreach (string nextStatus in nextStatuses)
                {
                    if (visited.Add(nextStatus))
                    {
                        previousStatus[nextStatus] = status;
                        pending.Enqueue(nextStatus);
                    }
                }
            }

            return new List<string>();
        }

        private Node Insert(Node node, MunicipalIssue issue)
        {
            if (node == null)
            {
                return new Node(issue);
            }

            int comparison = StringComparer.OrdinalIgnoreCase.Compare(issue.ReferenceNumber, node.Issue.ReferenceNumber);
            if (comparison < 0)
            {
                node.Left = Insert(node.Left, issue);
            }
            else
            {
                node.Right = Insert(node.Right, issue);
            }

            UpdateHeight(node);
            int balance = GetBalance(node);

            if (balance > 1)
            {
                if (StringComparer.OrdinalIgnoreCase.Compare(issue.ReferenceNumber, node.Left.Issue.ReferenceNumber) > 0)
                {
                    node.Left = RotateLeft(node.Left);
                }

                return RotateRight(node);
            }

            if (balance < -1)
            {
                if (StringComparer.OrdinalIgnoreCase.Compare(issue.ReferenceNumber, node.Right.Issue.ReferenceNumber) < 0)
                {
                    node.Right = RotateRight(node.Right);
                }

                return RotateLeft(node);
            }

            return node;
        }

        private void AddInOrder(Node node, List<MunicipalIssue> result)
        {
            if (node == null)
            {
                return;
            }

            AddInOrder(node.Left, result);
            result.Add(node.Issue);
            AddInOrder(node.Right, result);
        }

        private Node RotateLeft(Node node)
        {
            Node newRoot = node.Right;
            Node movedSubtree = newRoot.Left;
            newRoot.Left = node;
            node.Right = movedSubtree;
            UpdateHeight(node);
            UpdateHeight(newRoot);
            return newRoot;
        }

        private Node RotateRight(Node node)
        {
            Node newRoot = node.Left;
            Node movedSubtree = newRoot.Right;
            newRoot.Right = node;
            node.Left = movedSubtree;
            UpdateHeight(node);
            UpdateHeight(newRoot);
            return newRoot;
        }

        private void UpdateHeight(Node node)
        {
            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
        }

        private int GetBalance(Node node)
        {
            return node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
        }

        private int GetHeight(Node node)
        {
            return node == null ? 0 : node.Height;
        }

        private sealed class Node
        {
            public Node(MunicipalIssue issue)
            {
                Issue = issue;
                Height = 1;
            }

            public MunicipalIssue Issue { get; private set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public int Height { get; set; }
        }
    }
}
