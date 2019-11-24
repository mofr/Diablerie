
// It's generated file. DO NOT MODIFY IT!
class CharStatsInfoLoader : Datasheet.Loader<CharStatsInfo>
{
    private CharStatsInfoStartingItemLoader charstatsinfostartingitemloader = new CharStatsInfoStartingItemLoader();

    public void LoadRecord(ref CharStatsInfo record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.className);
                Datasheet.Parse(stream.NextString(), ref record.str);
                Datasheet.Parse(stream.NextString(), ref record.dex);
                Datasheet.Parse(stream.NextString(), ref record.energy);
                Datasheet.Parse(stream.NextString(), ref record.vit);
                Datasheet.Parse(stream.NextString(), ref record.tot);
                Datasheet.Parse(stream.NextString(), ref record.stamina);
                Datasheet.Parse(stream.NextString(), ref record.hpAdd);
                Datasheet.Parse(stream.NextString(), ref record.percentStr);
                Datasheet.Parse(stream.NextString(), ref record.percentDex);
                Datasheet.Parse(stream.NextString(), ref record.percentInt);
                Datasheet.Parse(stream.NextString(), ref record.percentVit);
                Datasheet.Parse(stream.NextString(), ref record.manaRegen);
                Datasheet.Parse(stream.NextString(), ref record.toHitFactor);
                Datasheet.Parse(stream.NextString(), ref record.walkVelocity);
                Datasheet.Parse(stream.NextString(), ref record.runVelocity);
                Datasheet.Parse(stream.NextString(), ref record.runDrain);
                Datasheet.Parse(stream.NextString(), ref record.comment);
                Datasheet.Parse(stream.NextString(), ref record.lifePerLevel);
                Datasheet.Parse(stream.NextString(), ref record.staminaPerLevel);
                Datasheet.Parse(stream.NextString(), ref record.manaPerLevel);
                Datasheet.Parse(stream.NextString(), ref record.lifePerVitality);
                Datasheet.Parse(stream.NextString(), ref record.staminPerVitality);
                Datasheet.Parse(stream.NextString(), ref record.manaPerMagic);
                Datasheet.Parse(stream.NextString(), ref record.statPerLevel);
                Datasheet.Parse(stream.NextString(), ref record.refWalk);
                Datasheet.Parse(stream.NextString(), ref record.refRun);
                Datasheet.Parse(stream.NextString(), ref record.refSwing);
                Datasheet.Parse(stream.NextString(), ref record.refSpell);
                Datasheet.Parse(stream.NextString(), ref record.refGetHit);
                Datasheet.Parse(stream.NextString(), ref record.refBow);
                Datasheet.Parse(stream.NextString(), ref record.blockFactor);
                Datasheet.Parse(stream.NextString(), ref record.startSkill);
                record.skills = new string[10];
                    Datasheet.Parse(stream.NextString(), ref record.skills[0]);
                    Datasheet.Parse(stream.NextString(), ref record.skills[1]);
                    Datasheet.Parse(stream.NextString(), ref record.skills[2]);
                    Datasheet.Parse(stream.NextString(), ref record.skills[3]);
                    Datasheet.Parse(stream.NextString(), ref record.skills[4]);
                    Datasheet.Parse(stream.NextString(), ref record.skills[5]);
                    Datasheet.Parse(stream.NextString(), ref record.skills[6]);
                    Datasheet.Parse(stream.NextString(), ref record.skills[7]);
                    Datasheet.Parse(stream.NextString(), ref record.skills[8]);
                    Datasheet.Parse(stream.NextString(), ref record.skills[9]);
                Datasheet.Parse(stream.NextString(), ref record.strAllSkills);
                Datasheet.Parse(stream.NextString(), ref record.strSkillTab1);
                Datasheet.Parse(stream.NextString(), ref record.strSkillTab2);
                Datasheet.Parse(stream.NextString(), ref record.strSkillTab3);
                Datasheet.Parse(stream.NextString(), ref record.strClassOnly);
                Datasheet.Parse(stream.NextString(), ref record.baseWClass);
                record.startingItems = new CharStatsInfo.StartingItem[10];
                    charstatsinfostartingitemloader.LoadRecord(ref record.startingItems[0], stream);
                    charstatsinfostartingitemloader.LoadRecord(ref record.startingItems[1], stream);
                    charstatsinfostartingitemloader.LoadRecord(ref record.startingItems[2], stream);
                    charstatsinfostartingitemloader.LoadRecord(ref record.startingItems[3], stream);
                    charstatsinfostartingitemloader.LoadRecord(ref record.startingItems[4], stream);
                    charstatsinfostartingitemloader.LoadRecord(ref record.startingItems[5], stream);
                    charstatsinfostartingitemloader.LoadRecord(ref record.startingItems[6], stream);
                    charstatsinfostartingitemloader.LoadRecord(ref record.startingItems[7], stream);
                    charstatsinfostartingitemloader.LoadRecord(ref record.startingItems[8], stream);
                    charstatsinfostartingitemloader.LoadRecord(ref record.startingItems[9], stream);
    }
}
