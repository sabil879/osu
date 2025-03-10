﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Database;
using osu.Game.Graphics;
using osu.Game.Input;
using osu.Game.Input.Bindings;
using osu.Game.IO;
using osu.Game.Online.API;
using osu.Game.Online.Chat;
using osu.Game.Overlays;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Scoring;
using osu.Game.Screens.Menu;
using osu.Game.Skinning;
using osu.Game.Utils;
using osuTK.Graphics;

namespace osu.Game.Tests.Visual.Navigation
{
    [TestFixture]
    public class TestSceneOsuGame : OsuTestScene
    {
        private IReadOnlyList<Type> requiredGameDependencies => new[]
        {
            typeof(OsuGame),
            typeof(SentryLogger),
            typeof(OsuLogo),
            typeof(IdleTracker),
            typeof(OnScreenDisplay),
            typeof(NotificationOverlay),
            typeof(BeatmapListingOverlay),
            typeof(DashboardOverlay),
            typeof(NewsOverlay),
            typeof(ChannelManager),
            typeof(ChatOverlay),
            typeof(SettingsOverlay),
            typeof(UserProfileOverlay),
            typeof(BeatmapSetOverlay),
            typeof(LoginOverlay),
            typeof(MusicController),
            typeof(AccountCreationOverlay),
            typeof(DialogOverlay),
            typeof(ScreenshotManager)
        };

        private IReadOnlyList<Type> requiredGameBaseDependencies => new[]
        {
            typeof(OsuGameBase),
            typeof(DatabaseContextFactory),
            typeof(Bindable<RulesetInfo>),
            typeof(IBindable<RulesetInfo>),
            typeof(Bindable<IReadOnlyList<Mod>>),
            typeof(IBindable<IReadOnlyList<Mod>>),
            typeof(LargeTextureStore),
            typeof(OsuConfigManager),
            typeof(SkinManager),
            typeof(ISkinSource),
            typeof(IAPIProvider),
            typeof(RulesetStore),
            typeof(FileStore),
            typeof(ScoreManager),
            typeof(BeatmapManager),
            typeof(SettingsStore),
            typeof(RulesetConfigCache),
            typeof(OsuColour),
            typeof(IBindable<WorkingBeatmap>),
            typeof(Bindable<WorkingBeatmap>),
            typeof(GlobalActionContainer),
            typeof(PreviewTrackManager),
        };

        private OsuGame game;

        [Resolved]
        private OsuGameBase gameBase { get; set; }

        [Resolved]
        private GameHost host { get; set; }

        [SetUpSteps]
        public void SetUpSteps()
        {
            AddStep("create game", () =>
            {
                game = new OsuGame();
                game.SetHost(host);

                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black,
                    },
                    game
                };
            });

            AddUntilStep("wait for load", () => game.IsLoaded);
        }

        [Test]
        public void TestNullRulesetHandled()
        {
            RulesetInfo ruleset = null;

            AddStep("store current ruleset", () => ruleset = Ruleset.Value);
            AddStep("set global ruleset to null value", () => Ruleset.Value = null);

            AddAssert("ruleset still valid", () => Ruleset.Value.Available);
            AddAssert("ruleset unchanged", () => ReferenceEquals(Ruleset.Value, ruleset));
        }

        [Test]
        public void TestUnavailableRulesetHandled()
        {
            RulesetInfo ruleset = null;

            AddStep("store current ruleset", () => ruleset = Ruleset.Value);
            AddStep("set global ruleset to invalid value", () => Ruleset.Value = new RulesetInfo
            {
                Name = "unavailable",
                Available = false,
            });

            AddAssert("ruleset still valid", () => Ruleset.Value.Available);
            AddAssert("ruleset unchanged", () => ReferenceEquals(Ruleset.Value, ruleset));
        }

        [Test]
        public void TestAvailableDependencies()
        {
            AddAssert("check OsuGame DI members", () =>
            {
                foreach (var type in requiredGameDependencies)
                {
                    if (game.Dependencies.Get(type) == null)
                        throw new InvalidOperationException($"{type} has not been cached");
                }

                return true;
            });

            AddAssert("check OsuGameBase DI members", () =>
            {
                foreach (var type in requiredGameBaseDependencies)
                {
                    if (gameBase.Dependencies.Get(type) == null)
                        throw new InvalidOperationException($"{type} has not been cached");
                }

                return true;
            });
        }
    }
}
