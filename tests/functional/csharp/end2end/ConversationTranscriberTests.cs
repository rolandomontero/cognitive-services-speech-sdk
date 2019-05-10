//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Conversation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicrosoftSpeechSDKSamples;

namespace Microsoft.CognitiveServices.Speech.Tests.EndToEnd
{
    using static ConversationTranscriberTestsHelper;
    using static SpeechRecognitionTestsHelper;

    [TestClass]
    public class ConversationTranscriberTests : RecognitionTestBase
    {
        private ConversationTranscriberTestsHelper helper;
        private static string conversationTranscriptionMultiAudioEndpoint;

        // Copy the Signature value from the Response body after calling the RESTFUL API at https://signature.centralus.cts.speech.microsoft.com
        // Voice signature format example: { "Version": <Numeric value>, "Tag": "string", "Data": "string" }
        private const string voiceSignatureKatie = "{ \"Version\": 0, \"Tag\": \"VtZQ7sJp8np3AxQC+87WYyBHWsZohWFBN0zgWzzOnpw=\", \"Data\": \"BhRRgDCrg6ij5fylg5Jpf5ZW/o/uDWi199DYBbfL1Qdspj77qiuvsVKzG2g5Z9jxKtfdwtkKtaDxog9O6pGD7Ot/8mM1jUtt6LKNz4H9EFvznV/dlFk2Oisg8ZKx/RBlNFMYJkQJnxT/zLfhNWiIF5Y97jH4sgRh2orDg6/567FGktAgcESAbiDx1e7tf0TTLdwijw4p1vJ3qJ2cSCdNbXE9KeUd8sClQLDheCPo+et3rMs5W+Rju3W1SJE6ru9gAoH88CyAfI80+ysAecH3GPJYM+j1uhvmWoKIrSfS40BYOe6AUgLNb3a4Pue4oGAmuAyWfwpP1uezleSanpJc73HT91n2UsaHrQj5eK6uuBCjwmu+JI3FT+Vo6JhAARHecdb70U1wyW9vv5t0Q3tV1WNiN/30qSVydDtyrSVpgBiIwlj41Ua22JJCOMrPl7NKLnFmeZ4Hi4aIKoHAxDvrApteL60sxLX/ADAtYCB3Y6iagDyR1IOsIlbaPhL0rQDnC/0z65k7BDekietFNzvvPVoIwJ26GHrXFYYwZe3alVvCsXTpZGBknvSiaCalrixnyGqYo0nG/cd/LodEEIht/PWoFkNlbABqHMbToeI/6j+ICKuVJgTDtcsDQiWKSvrQp9kTSv+cF3LyPVkbks0JvbQhj4AkAv7Rvymgnsu6o8gGyxTX0kxujqCMPCzgFrVd\"}";
        private const string voiceSignatureSteve = "{ \"Version\": 0, \"Tag\": \"HbIvzbfAWjeR/3R+WvUEoeid1AbDaHNOMWItgs7mTxc=\", \"Data\": \"DizY04Z7PH/sYu2Yw2EcL4Mvj1GnEDOWJ/DhXHGdQJsQ8/zDc13z1cwllbEo5OSr3oGoKEHLV95OUA6PgksZzvTkf42iOFEv3yifUNfYkZuIzStZoDxWu1H1BoFBejqzSpCYyvqLwilWOyUeMn+z+E4+zXjqHUCyYJ/xf0C3+58kCbmyA55yj7YZ6OtMVyFmfT2GLiXr4YshUB14dgwl3Y08SRNavnG+/QOs+ixf3UoZ6BC1VZcVQnC2tn2FB+8v6ehnIOTQedo++6RWIB0RYmQ8VaEeI0E4hkpA1OxQ9f2gBVtw3KZXWSWBz8sXig2igpwMsQoFRmmIOGsu+p6tM8/OThQpARZ7OyAxsurzmaSGZAaXYt0YwMdIIXKeDBF6/KnUyw+NNzku1875u2Fde/bxgVvCOwhrLPPuu/RZUeAkwVQge7nKYNW5YjDcz8mfg4LfqWEGOVCcmf2IitQtcIEjY3MwLVNvsAB6GT2es1/1QieCfQKy/Tdu8IUfEvekwSCxSlWhfVrLjRhGeWa9idCjsngQbNkqYUNdnIlidkn2DC4BavSTYXR5lVxV4SR/Vvj8h4N5nP/URPDhkzl7n7Tqd4CGFZDzZzAr7yRo3PeUBX0CmdrKLW3+GIXAdvpFAx592pB0ySCv5qBFhJNErEINawfGcmeWZSORxJg1u+agj51zfTdrHZeugFcMs6Be\"}";

        [ClassInitialize]
        public static void TestClassinitialize(TestContext context)
        {
            BaseClassInit(context);
            conversationTranscriptionMultiAudioEndpoint = conversationTranscriptionEndpoint + "/multiaudio";
        }

        [TestInitialize]
        public void Initialize()
        {
            helper = new ConversationTranscriberTestsHelper();
        }

        [TestMethod, TestCategory(TestCategory.LongRunning)]
        public void ConversationIdWithChinese()
        {
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);
            var audioInput = AudioConfig.FromWavFileInput(TestData.English.Weather8Channels.AudioFile);
            using (var conversationTranscriber = TrackSessionId(new ConversationTranscriber(config, audioInput)))
            {
                //the UTF8 decoding of 的 is \xe7\x9a\x84, which will be shown in the debugger in the C++ side.
                string myConversationId = "的";
                conversationTranscriber.ConversationId = myConversationId;
                var gotId = conversationTranscriber.ConversationId;
                Assert.AreEqual(myConversationId, gotId);
            }
        }

        [TestMethod, TestCategory(TestCategory.LongRunning)]
        public void ConversationIdWithAnsiOnly()
        {
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);
            var audioInput = AudioConfig.FromWavFileInput(TestData.English.Weather8Channels.AudioFile);
            using (var conversationTranscriber = TrackSessionId(new ConversationTranscriber(config, audioInput)))
            {
                string myConversationId = "123 456";
                conversationTranscriber.ConversationId = myConversationId;
                var gotId = conversationTranscriber.ConversationId;
                Assert.AreEqual(myConversationId, gotId);
            }
        }

        [TestMethod, TestCategory(TestCategory.LongRunning)]
        public void ConversationCreateUsers()
        {
            string myId = "xyz@example.com";
            var user = User.FromUserId(myId);
            Assert.AreEqual(myId, user.UserId);
        }

        [TestMethod, TestCategory(TestCategory.LongRunning)]
        public void ConversationCreateParticipantWithValidPreferredLanguageAndVoiceSignature()
        {
            bool exception = false;
            try
            {
                var participant = Participant.From("xyz@example.com", "zh-cn", voiceSignatureKatie);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Got Exception: " + ex.Message.ToString());
                exception = true;
            }
            Assert.AreEqual(exception, false);
        }

        [TestMethod, TestCategory(TestCategory.LongRunning)]
        public void ConversationCreateParticipantWithEmptyPreferredLanguage()
        {
            bool exception = false;
            try
            {
                var participant = Participant.From("xyz@example.com", "", voiceSignatureKatie);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Got Exception: " + ex.Message.ToString());
                exception = true;
            }
            Assert.AreEqual(exception, false);
        }

        [TestMethod, TestCategory(TestCategory.LongRunning)]
        public void ConversationCreateParticipantWithInvalidPreferredLanguage()
        {
            bool exception = false;
            try
            {
                var participant = Participant.From("xyz@example.com", "invalid", voiceSignatureKatie);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Got Exception: " + ex.Message.ToString());
                exception = true;
            }
            Assert.AreEqual(exception, false);
        }

        [TestMethod, TestCategory(TestCategory.LongRunning)]
        public void ConversationCreateParticipantWithEmptyVoiceSignature()
        {
            bool exception = false;
            try
            {
                var participant = Participant.From("xyz@example.com", "zh-cn", string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Got Exception: " + ex.Message.ToString());
                exception = true;
            }
            Assert.AreEqual(exception, false);
        }

        [TestMethod, TestCategory(TestCategory.LongRunning)]
        public void ConversationCreateParticipantWithInvalidVoiceSignature()
        {
            bool exception = false;
            try
            {
                var participant2 = Participant.From("xyz@example.com", "", "1.1, 2.2");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Got Exception: " + ex.Message.ToString());
                exception = true;
            }
            Assert.AreEqual(exception, true);
        }

        [TestMethod]
        public async Task ConversationAddParticipant()
        {
            var config = SpeechConfig.FromEndpoint(new Uri(conversationTranscriptionMultiAudioEndpoint), conversationTranscriptionPPEKey);
            var audioInput = AudioConfig.FromWavFileInput(TestData.English.TranscriberAudioData.TwoSpeakersAudio);
            using (var conversationTranscriber = TrackSessionId(new ConversationTranscriber(config, audioInput)))
            {
                conversationTranscriber.ConversationId = "TestCreatingParticipantByUserClass";

                conversationTranscriber.AddParticipant("OneUserByUserId");

                var user = User.FromUserId("CreateUserFromId and then add it");
                conversationTranscriber.AddParticipant(user);

                // Voice signature format as specified here https://aka.ms/cts/signaturegenservice
                string voice = voiceSignatureKatie;
                var participant = Participant.From("userIdForParticipant", "en-us", voice);
                conversationTranscriber.AddParticipant(participant);

                var result = await helper.GetFirstRecognizerResult(conversationTranscriber);
                AssertMatching(TestData.English.TranscriberAudioData.Utterance, result);
            }
        }

        [TestMethod]
        public async Task ConversationAddParticipantFromSubscription()
        {
            var config = SpeechConfig.FromSubscription(conversationTranscriptionPRODKey, speechRegionForConversationTranscription);
            var audioInput = AudioConfig.FromWavFileInput(TestData.English.TranscriberAudioData.TwoSpeakersAudio);
            using (var conversationTranscriber = TrackSessionId(new ConversationTranscriber(config, audioInput)))
            {
                conversationTranscriber.ConversationId = "TestCreatingParticipantByUserClass";

                conversationTranscriber.AddParticipant("OneUserByUserId");

                var user = User.FromUserId("CreateUserFromId and then add it");
                conversationTranscriber.AddParticipant(user);

                // Voice signature format as specified here https://aka.ms/cts/signaturegenservice
                string voice = voiceSignatureKatie;
                var participant = Participant.From("userIdForParticipant", "en-us", voice);
                conversationTranscriber.AddParticipant(participant);

                var result = await helper.GetFirstRecognizerResult(conversationTranscriber);
                AssertMatching(TestData.English.TranscriberAudioData.Utterance, result);
            }
        }

        [TestMethod, TestCategory(TestCategory.LongRunning)]
        public async Task ConversationRemoveParticipant()
        {
            var config = SpeechConfig.FromEndpoint(new Uri(conversationTranscriptionMultiAudioEndpoint), conversationTranscriptionPPEKey);
            var audioInput = AudioConfig.FromWavFileInput(TestData.English.TranscriberAudioData.TwoSpeakersAudio);
            using (var conversationTranscriber = TrackSessionId(new ConversationTranscriber(config, audioInput)))
            {
                conversationTranscriber.ConversationId = "TestCreatingParticipantByUserClass";
                bool exception = false;
                try
                {
                    conversationTranscriber.RemoveParticipant("NoneExist");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Got Exception: " + ex.Message.ToString());
                    exception = true;
                }
                Assert.AreEqual(exception, true);

                conversationTranscriber.AddParticipant("OneUserByUserId");
                conversationTranscriber.RemoveParticipant("OneUserByUserId");

                var user = User.FromUserId("user object created from User.FromUserId");
                conversationTranscriber.AddParticipant(user);
                conversationTranscriber.RemoveParticipant(user);

                // Voice signature format as specified here https://aka.ms/cts/signaturegenservice
                var participant = Participant.From("userIdForParticipant", "en-us", voiceSignatureKatie);
                conversationTranscriber.AddParticipant(participant);
                conversationTranscriber.RemoveParticipant(participant);

                var result = await helper.GetFirstRecognizerResult(conversationTranscriber);
                AssertMatching(TestData.English.TranscriberAudioData.Utterance, result);
            }
        }

        [TestMethod, TestCategory(TestCategory.LongRunning)]
        public async Task ConversationPushStream()
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromEndpoint(new Uri(conversationTranscriptionMultiAudioEndpoint), conversationTranscriptionPPEKey);
            var stopRecognition = new TaskCompletionSource<int>();
            string recoResult = string.Empty;

            using (var pushStream = AudioInputStream.CreatePushStream())
            {
                using (var audioInput = AudioConfig.FromStreamInput(pushStream))
                {
                    using (var conversationTranscriber = TrackSessionId(new ConversationTranscriber(config, audioInput)))
                    {
                        conversationTranscriber.ConversationId = "ConversationPushStreamTest";

                        conversationTranscriber.AddParticipant("xyz@example.com");

                        conversationTranscriber.Recognizing += (s, e) =>
                        {
                            Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                        };

                        conversationTranscriber.Recognized += (s, e) =>
                        {
                            if (e.Result.Reason == ResultReason.RecognizedSpeech)
                            {
                                Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                                if (string.IsNullOrEmpty(recoResult))
                                {
                                    recoResult = e.Result.Text;
                                }
                            }
                            else if (e.Result.Reason == ResultReason.NoMatch)
                            {
                                Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                            }
                        };

                        conversationTranscriber.Canceled += (s, e) =>
                        {
                            Console.WriteLine($"CANCELED: Reason={e.Reason}");

                            if (e.Reason == CancellationReason.Error)
                            {
                                Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                                Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                            }

                            stopRecognition.TrySetResult(0);
                        };

                        conversationTranscriber.SessionStarted += (s, e) =>
                        {
                            Console.WriteLine("\nSession started event.");
                        };

                        conversationTranscriber.SessionStopped += (s, e) =>
                        {
                            Console.WriteLine("\nSession stopped event.");
                            Console.WriteLine("\nStop recognition.");
                            stopRecognition.TrySetResult(0);
                        };

                        // Starts continuous recognition. Uses StopTranscribingAsync() to stop transcribing.
                        await conversationTranscriber.StartTranscribingAsync().ConfigureAwait(false);

                        // open and read the wave file and push the buffers into the recognizer
                        using (var reader = Util.CreateWavReader(TestData.English.Weather8Channels.AudioFile))
                        {
                            byte[] buffer = new byte[3200];
                            while (true)
                            {
                                var readSamples = reader.Read(buffer, (uint)buffer.Length);
                                if (readSamples == 0)
                                {
                                    break;
                                }
                                pushStream.Write(buffer, readSamples);
                                pushStream.SetProperty(PropertyId.ConversationTranscribingService_DataBufferTimeStamp, "fakeTimeStamp");
                                pushStream.SetProperty("DataChunk_SpeakerId", "fakeSpeakerId");
                            }
                        }
                        pushStream.Close();

                        // Waits for completion.
                        // Use Task.WaitAny to keep the task rooted.
                        Task.WaitAny(new[] { stopRecognition.Task });

                        await conversationTranscriber.StopTranscribingAsync().ConfigureAwait(false);
                        Assert.IsFalse(string.IsNullOrEmpty(recoResult));
                    }
                }
            }
        }

        [TestMethod, TestCategory(TestCategory.LongRunning)]
        public async Task ConversationPullStream()
        {
            var config = SpeechConfig.FromEndpoint(new Uri(conversationTranscriptionMultiAudioEndpoint), conversationTranscriptionPPEKey);
            var stopRecognition = new TaskCompletionSource<int>();
            bool bGotReco = false;

            using (var audioInput = Util.OpenWavFile(TestData.English.TranscriberAudioData.TwoSpeakersAudio))
            {
                using (var conversationTranscriber = TrackSessionId(new ConversationTranscriber(config, audioInput)))
                {
                    conversationTranscriber.ConversationId = "ConversationPullStreamTest";
                    conversationTranscriber.AddParticipant("xyz@example.com");

                    conversationTranscriber.Recognizing += (s, e) =>
                    {
                        Console.WriteLine($"RECOGNIZING: Text={e.Result.Text} SpeakerId={e.Result.UserId}");
                    };

                    conversationTranscriber.Recognized += (s, e) =>
                    {
                        if (e.Result.Reason == ResultReason.RecognizedSpeech)
                        {
                            Console.WriteLine($"RECOGNIZED: Text={e.Result.Text} SpeakerId={e.Result.UserId}");
                            bGotReco = true;
                        }
                        else if (e.Result.Reason == ResultReason.NoMatch)
                        {
                            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        }
                    };

                    conversationTranscriber.Canceled += (s, e) =>
                    {
                        Console.WriteLine($"CANCELED: Reason={e.Reason}");

                        if (e.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }

                        stopRecognition.TrySetResult(0);
                    };

                    conversationTranscriber.SessionStarted += (s, e) =>
                    {
                        Console.WriteLine("\nSession started event.");
                    };

                    conversationTranscriber.SessionStopped += (s, e) =>
                    {
                        Console.WriteLine("\nSession stopped event.");
                        Console.WriteLine("\nStop recognition.");
                        stopRecognition.TrySetResult(0);
                    };

                    // Starts continuous recognition. Uses StopTranscribingAsync() to stop transcribing.
                    await conversationTranscriber.StartTranscribingAsync().ConfigureAwait(false);

                    // Waits for completion.
                    // Use Task.WaitAny to keep the task rooted.
                    Task.WaitAny(new[] { stopRecognition.Task });

                    await conversationTranscriber.StopTranscribingAsync().ConfigureAwait(false);
                    Assert.IsTrue(bGotReco);
                }
            }
        }

        [TestMethod, TestCategory(TestCategory.LongRunning)]
        public async Task ConversationDetailedOutput()
        {
            var config = SpeechConfig.FromEndpoint(new Uri(conversationTranscriptionMultiAudioEndpoint), conversationTranscriptionPPEKey);
            config.OutputFormat = OutputFormat.Detailed;
            var audioInput = AudioConfig.FromWavFileInput(TestData.English.Weather8Channels.AudioFile);
            using (var conversationTranscriber = TrackSessionId(new ConversationTranscriber(config, audioInput)))
            {
                conversationTranscriber.ConversationId = "TestCreatingParticipantByUserClass";
                await helper.CompleteContinuousRecognition(conversationTranscriber);
                var connectionUrl = conversationTranscriber.Properties.GetProperty(PropertyId.SpeechServiceConnection_Url);
                // Currently we do not have endpoint ready that supports detailed conversation transcription, so we only check connection URL for now. 
                Assert.IsTrue(connectionUrl.Contains("format=detailed"), "mismatch initialSilencetimeout in " + connectionUrl);
            }
        }
    }
}
