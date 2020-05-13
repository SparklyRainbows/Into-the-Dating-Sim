using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationshipManager {
    private static List<Relationship> relationships = new List<Relationship>() {
        new Relationship(UnitPair.PHYNNEMINKEE),
        new Relationship(UnitPair.VHALLPHYNNE),
        new Relationship(UnitPair.MINKEEVHALL)
    };

    public static Relationship GetRelationshipBetween(UnitID a, UnitID b) {
        UnitPair pair = GetRelationshipPair(a, b);

        foreach (Relationship r in relationships) {
            if (r.pair == pair) {
                return r;
            }
        }

        Debug.LogError("Couldn't find relationship between " + a + " and " + b);
        return null;
    }

    public static UnitPair GetRelationshipPair(UnitID a, UnitID b) {
        if ((a == UnitID.MINKEE && b == UnitID.PHYNNE) || (b == UnitID.MINKEE && a == UnitID.PHYNNE)) {
            return UnitPair.PHYNNEMINKEE;
        }
        if ((a == UnitID.PHYNNE && b == UnitID.VHALL) || (b == UnitID.PHYNNE && a == UnitID.VHALL)) {
            return UnitPair.VHALLPHYNNE;
        }
        if ((a == UnitID.VHALL && b == UnitID.MINKEE) || (b == UnitID.VHALL && a == UnitID.MINKEE)) {
            return UnitPair.MINKEEVHALL;
        }

        return UnitPair.MINKEEVHALL;
    }

    public static string AsString(RelationshipRank rank) {
        switch (rank) {
            case RelationshipRank.SPICY:
                return "S";
            case RelationshipRank.APPETIZING:
                return "A";
            case RelationshipRank.BLAND:
                return "B";
            case RelationshipRank.CRUDE:
                return "C";
        }
        return "";
    }
}

public class Relationship {
    private RelationshipRank rank;
    private int supportPoints;

    public UnitPair pair;

    private static Dictionary<int, RelationshipRank> pointsPerRank = new Dictionary<int, RelationshipRank>() {
        {1, RelationshipRank.CRUDE },
        {6, RelationshipRank.BLAND },
        {12, RelationshipRank.APPETIZING },
        {19, RelationshipRank.SPICY }
    };
    private static Dictionary<RelationshipRank, int> attackPerRank = new Dictionary<RelationshipRank, int>() {
        { RelationshipRank.CRUDE, 0},
        { RelationshipRank.BLAND, 1},
        { RelationshipRank.APPETIZING, 1},
        { RelationshipRank.SPICY, 2}
    };

    private Dictionary<RelationshipRank, bool> relationshipAttained;

    public Relationship(UnitPair pair) {
        this.pair = pair;
        
        rank = RelationshipRank.NONE;
        supportPoints = 0;

        relationshipAttained = new Dictionary<RelationshipRank, bool>() {
            { RelationshipRank.CRUDE, false},
            { RelationshipRank.BLAND, false},
            { RelationshipRank.APPETIZING, false},
            { RelationshipRank.SPICY, false}
        };
    }

    public void IncreaseSupport(int amount) {
        supportPoints += amount;
    }

    public void IncreaseSupport() {
        IncreaseSupport(1);
    }

    public RelationshipRank GetNextRank() {
        return GetHighestRank();
    }

    public RelationshipRank GetRank() {
        return rank;
    }

    public void FinishSupportConversation() {
        rank = GetHighestRank();
        relationshipAttained[rank] = true;
    }

    public bool ReachedNextRelationship() {
        RelationshipRank nextRank = GetHighestRank();
        return nextRank != rank;
    }

    private RelationshipRank GetHighestRank() {
        foreach (int threshold in pointsPerRank.Keys) {
            if (supportPoints >= threshold && !relationshipAttained[pointsPerRank[threshold]]) {
                return pointsPerRank[threshold];
            }
        }

        return rank;
    }

    public static int GetAttackBonus(RelationshipRank r) {
        return attackPerRank[r];
    }
}

public enum RelationshipRank {
    SPICY = 4,
    APPETIZING = 3,
    BLAND = 2,
    CRUDE = 1,
    NONE = 0
}

public enum UnitPair {
    PHYNNEMINKEE,
    MINKEEVHALL,
    VHALLPHYNNE
}