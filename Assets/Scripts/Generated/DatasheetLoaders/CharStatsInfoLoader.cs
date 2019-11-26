
// It's generated file. DO NOT MODIFY IT!
class CharStatsInfoLoader : Datasheet.Loader<CharStatsInfo>
{
    private CharStatsInfoStartingItemLoader charstatsinfostartingitemloader = new CharStatsInfoStartingItemLoader();

    public void LoadRecord(ref CharStatsInfo record, Datasheet.Stream stream)
    {
                stream.Read(ref record.className);
                stream.Read(ref record.str);
                stream.Read(ref record.dex);
                stream.Read(ref record.energy);
                stream.Read(ref record.vit);
                stream.Read(ref record.tot);
                stream.Read(ref record.stamina);
                stream.Read(ref record.hpAdd);
                stream.Read(ref record.percentStr);
                stream.Read(ref record.percentDex);
                stream.Read(ref record.percentInt);
                stream.Read(ref record.percentVit);
                stream.Read(ref record.manaRegen);
                stream.Read(ref record.toHitFactor);
                stream.Read(ref record.walkVelocity);
                stream.Read(ref record.runVelocity);
                stream.Read(ref record.runDrain);
                stream.Read(ref record.comment);
                stream.Read(ref record.lifePerLevel);
                stream.Read(ref record.staminaPerLevel);
                stream.Read(ref record.manaPerLevel);
                stream.Read(ref record.lifePerVitality);
                stream.Read(ref record.staminPerVitality);
                stream.Read(ref record.manaPerMagic);
                stream.Read(ref record.statPerLevel);
                stream.Read(ref record.refWalk);
                stream.Read(ref record.refRun);
                stream.Read(ref record.refSwing);
                stream.Read(ref record.refSpell);
                stream.Read(ref record.refGetHit);
                stream.Read(ref record.refBow);
                stream.Read(ref record.blockFactor);
                stream.Read(ref record.startSkill);
                record.skills = new string[10];
                    stream.Read(ref record.skills[0]);
                    stream.Read(ref record.skills[1]);
                    stream.Read(ref record.skills[2]);
                    stream.Read(ref record.skills[3]);
                    stream.Read(ref record.skills[4]);
                    stream.Read(ref record.skills[5]);
                    stream.Read(ref record.skills[6]);
                    stream.Read(ref record.skills[7]);
                    stream.Read(ref record.skills[8]);
                    stream.Read(ref record.skills[9]);
                stream.Read(ref record.strAllSkills);
                stream.Read(ref record.strSkillTab1);
                stream.Read(ref record.strSkillTab2);
                stream.Read(ref record.strSkillTab3);
                stream.Read(ref record.strClassOnly);
                stream.Read(ref record.baseWClass);
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
