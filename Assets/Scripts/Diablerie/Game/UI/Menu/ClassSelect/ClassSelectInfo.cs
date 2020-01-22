using System.Collections.Generic;
using Diablerie.Engine;
using UnityEngine;

namespace Diablerie.Game.UI.Menu.ClassSelect
{
    public class ClassSelectInfo
    {
        public string ClassName { get; set; }
        
        public string Description { get; set; }
        
        public string Token { get; set; }
        
        public Material OverlayMaterial { get; set; }
        
        public int BaseSortingOrder { get; set; }
        
        public int OverlaySortingOrder { get; set; }
        
        public bool HasBackTransitionOverlay { get; set; }
        
        public bool HasFrontTransitionOverlay { get; set; }
        
        public bool HasFrontIdleOverlay { get; set; }
        
        public int BackIdleFps { get; set; }
        
        public int FrontIdleFps { get; set; }

        public static ClassSelectInfo Find(string className)
        {
            return ClassAnimationInfos.Find(item => item.ClassName == className);
        }
        
        private static readonly List<ClassSelectInfo> ClassAnimationInfos = new List<ClassSelectInfo>
        {
            new ClassSelectInfo
            {
                ClassName = "Amazon",
                Description = "Skilled with the spear and the bow, she is a very versatile fighter.",
                Token = "AM",
                OverlayMaterial = Materials.normal,
                BaseSortingOrder = 12,
                OverlaySortingOrder = 0,
                HasBackTransitionOverlay = false,
                HasFrontTransitionOverlay = false,
                HasFrontIdleOverlay = false,
                BackIdleFps = 10,
                FrontIdleFps = 15
            },
            new ClassSelectInfo
            {
                ClassName = "Assassin",
                Description = "Schooled in the Martial Arts, her mind and body are deadly weapons.",
                Token = "AS",
                OverlayMaterial = Materials.normal,
                BaseSortingOrder = 14,
                OverlaySortingOrder = 0,
                HasBackTransitionOverlay = false,
                HasFrontTransitionOverlay = false,
                HasFrontIdleOverlay = false,
                BackIdleFps = 10,
                FrontIdleFps = 15
            }, 
            new ClassSelectInfo
            {
                ClassName = "Necromancer",
                Description = "Summoning undead minions and cursing his enemies are his specialities.",
                Token = "NE",
                OverlayMaterial = Materials.softAdditive,
                BaseSortingOrder = 12,
                OverlaySortingOrder = -1,
                HasBackTransitionOverlay = true,
                HasFrontTransitionOverlay = true,
                HasFrontIdleOverlay = true,
                BackIdleFps = 10,
                FrontIdleFps = 15
            }, 
            new ClassSelectInfo
            {
                ClassName = "Barbarian",
                Description = "He is unequaled in close-quarters combat and mastery of weapons.",
                Token = "BA",
                OverlayMaterial = Materials.normal,
                BaseSortingOrder = 14,
                OverlaySortingOrder = 1,
                HasBackTransitionOverlay = false,
                HasFrontTransitionOverlay = true,
                HasFrontIdleOverlay = false,
                BackIdleFps = 10,
                FrontIdleFps = 20
            }, 
            new ClassSelectInfo
            {
                ClassName = "Paladin",
                Description = "He is a natural party leader, holy man, and blessed warrior.",
                Token = "PA",
                OverlayMaterial = Materials.normal,
                BaseSortingOrder = 16,
                OverlaySortingOrder = 1,
                HasBackTransitionOverlay = false,
                HasFrontTransitionOverlay = true,
                HasFrontIdleOverlay = false,
                BackIdleFps = 10,
                FrontIdleFps = 15
            }, 
            new ClassSelectInfo
            {
                ClassName = "Sorceress",
                Description = "She has mastered the elemental magicks - fire, lightning, and ice.",
                Token = "SO",
                OverlayMaterial = Materials.normal,
                BaseSortingOrder = 12,
                OverlaySortingOrder = -1,
                HasBackTransitionOverlay = true,
                HasFrontTransitionOverlay = true,
                HasFrontIdleOverlay = true,
                BackIdleFps = 10,
                FrontIdleFps = 15
            }, 
            new ClassSelectInfo
            {
                ClassName = "Druid",
                Description = "Commanding the forces of nature, he summons wild beasts and raging storms to his side.",
                Token = "DZ",
                OverlayMaterial = Materials.normal,
                BaseSortingOrder = 12,
                OverlaySortingOrder = 0,
                HasBackTransitionOverlay = false,
                HasFrontTransitionOverlay = false,
                HasFrontIdleOverlay = false,
                BackIdleFps = 10,
                FrontIdleFps = 15
            }
        };
    }
}


