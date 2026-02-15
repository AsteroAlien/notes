using System;
using System.Collections.Generic;

namespace Efscaffold.Entities;

public partial class Note
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Priority { get; set; }

    public bool IsComplete { get; set; }
}
