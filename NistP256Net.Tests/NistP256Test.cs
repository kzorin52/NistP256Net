using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NistP256Net;

namespace NistP256Net.Tests;

[TestClass]
[TestSubject(typeof(NistP256))]
public class NistP256Test
{
    private static readonly ReadOnlyMemory<byte> PrivateKey = new(Convert.FromHexString("ac5b6284de78a208ebbb0f23a3438f2fab98331b43682235e3290ccca023e2a8"));
    private static readonly byte[] PublicKey = Convert.FromHexString("03e263598d507e1de7bdcfb5d560497fdc2a77948e1955ba70db4a24d2554fab5e");

    [TestMethod]
    public void TestPrvToPub()
    {
        Assert.IsTrue(PublicKey.SequenceEqual(NistP256.GetPublicKey(PrivateKey.Span)));
    }
}