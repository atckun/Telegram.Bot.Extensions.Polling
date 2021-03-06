﻿using System.Threading.Tasks;
using Xunit;

namespace Telegram.Bot.Extensions.Polling.Tests.YieldingUpdateReceivers
{
    public class BlockingUpdateReceiverTests
    {
        [Fact]
        public async Task BlocksWhileProcessingAsync()
        {
            var mockClient = new MockTelegramBotClient("test", "break", "test");
            var receiver = new BlockingUpdateReceiver(mockClient);

            Assert.Equal(3, mockClient.MessageGroupsLeft);

            await foreach (var update in receiver.YieldUpdatesAsync())
            {
                if (update.Message.Text == "break")
                    break;
            }

            Assert.Equal(1, mockClient.MessageGroupsLeft);
        }

        [Fact]
        public async Task ReturnsReceivedPendingUpdates()
        {
            var mockClient = new MockTelegramBotClient("foo-bar", "123");
            var receiver = new BlockingUpdateReceiver(mockClient);

            Assert.Equal(2, mockClient.MessageGroupsLeft);
            Assert.Equal(0, receiver.PendingUpdates);

            await foreach (var update in receiver.YieldUpdatesAsync())
            {
                Assert.Equal("foo", update.Message.Text);
                break;
            }

            Assert.Equal(1, mockClient.MessageGroupsLeft);
            Assert.Equal(1, receiver.PendingUpdates);

            await foreach (var update in receiver.YieldUpdatesAsync())
            {
                Assert.Equal("bar", update.Message.Text);
                break;
            }

            Assert.Equal(1, mockClient.MessageGroupsLeft);
            Assert.Equal(0, receiver.PendingUpdates);

            await foreach (var update in receiver.YieldUpdatesAsync())
            {
                Assert.Equal("123", update.Message.Text);
                break;
            }

            Assert.Equal(0, mockClient.MessageGroupsLeft);
            Assert.Equal(0, receiver.PendingUpdates);
        }
    }
}
