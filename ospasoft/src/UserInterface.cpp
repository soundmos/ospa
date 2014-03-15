// generated by Fast Light User Interface Designer (fluid) version 1.0302

#include "UserInterface.h"

NewTaskWindow::NewTaskWindow() {
  { FlWindow = new Fl_Double_Window(330, 145, "New Task");
    FlWindow->labelsize(12);
    FlWindow->user_data((void*)(this));
    FlWindow->align(Fl_Align(FL_ALIGN_CLIP|FL_ALIGN_INSIDE));
    { TaskSymbolTxt = new Fl_Input(150, 10, 170, 25, "Task symbol: ");
      TaskSymbolTxt->labelsize(12);
      TaskSymbolTxt->textfont(4);
    } // Fl_Input* TaskSymbolTxt
    { ActivationCmb = new Fl_Choice(150, 40, 170, 25, "Activation: ");
      ActivationCmb->down_box(FL_BORDER_BOX);
      ActivationCmb->labelsize(12);
      ActivationCmb->textsize(12);
    } // Fl_Choice* ActivationCmb
    { IntervalNum = new Fl_Spinner(150, 70, 65, 25, "Interval (ms): ");
      IntervalNum->labelsize(12);
      IntervalNum->textfont(4);
      IntervalNum->deactivate();
    } // Fl_Spinner* IntervalNum
    { OkBtn = new Fl_Return_Button(128, 110, 109, 25, "Create task");
      OkBtn->labelsize(12);
    } // Fl_Return_Button* OkBtn
    { CancelBtn = new Fl_Button(247, 110, 73, 25, "Cancel");
      CancelBtn->labelsize(12);
    } // Fl_Button* CancelBtn
    FlWindow->set_modal();
    FlWindow->end();
  } // Fl_Double_Window* FlWindow
}

Fl_Menu_Item ProgramWindow::menu_MenuBar[] = {
 {"&File", 0,  0, 0, 64, FL_NORMAL_LABEL, 0, 12, 0},
 {"&New program ", 0x4006e,  0, 0, 0, FL_NORMAL_LABEL, 0, 12, 0},
 {0,0,0,0,0,0,0,0,0},
 {"&Edit", 0,  0, 0, 64, FL_NORMAL_LABEL, 0, 12, 0},
 {0,0,0,0,0,0,0,0,0},
 {"&Program", 0,  0, 0, 64, FL_NORMAL_LABEL, 0, 12, 0},
 {0,0,0,0,0,0,0,0,0},
 {"&Tools", 0,  0, 0, 64, FL_NORMAL_LABEL, 0, 12, 0},
 {0,0,0,0,0,0,0,0,0},
 {"&Help", 0,  0, 0, 64, FL_NORMAL_LABEL, 0, 12, 0},
 {"&About OSPASOFT ", 0,  0, 0, 0, FL_NORMAL_LABEL, 0, 12, 0},
 {0,0,0,0,0,0,0,0,0},
 {0,0,0,0,0,0,0,0,0}
};
Fl_Menu_Item* ProgramWindow::FileMnu = ProgramWindow::menu_MenuBar + 0;
Fl_Menu_Item* ProgramWindow::NewProgramMnu = ProgramWindow::menu_MenuBar + 1;
Fl_Menu_Item* ProgramWindow::EditMnu = ProgramWindow::menu_MenuBar + 3;
Fl_Menu_Item* ProgramWindow::ProgramMnu = ProgramWindow::menu_MenuBar + 5;
Fl_Menu_Item* ProgramWindow::ToolsMnu = ProgramWindow::menu_MenuBar + 7;
Fl_Menu_Item* ProgramWindow::HelpMnu = ProgramWindow::menu_MenuBar + 9;
Fl_Menu_Item* ProgramWindow::AboutMnu = ProgramWindow::menu_MenuBar + 10;

ProgramWindow::ProgramWindow() {
  { FlWindow = new Fl_Double_Window(390, 410, "Program - OSPASOFT");
    FlWindow->labelsize(13);
    FlWindow->user_data((void*)(this));
    FlWindow->align(Fl_Align(FL_ALIGN_CLIP|FL_ALIGN_INSIDE));
    { Fl_Group* o = new Fl_Group(0, 30, 394, 371);
      o->labelsize(12);
      o->end();
      Fl_Group::current()->resizable(o);
    } // Fl_Group* o
    { MenuBar = new Fl_Menu_Bar(0, 0, 394, 25);
      MenuBar->box(FL_THIN_UP_BOX);
      MenuBar->labelsize(12);
      MenuBar->textsize(11);
      MenuBar->menu(menu_MenuBar);
    } // Fl_Menu_Bar* MenuBar
    { Fl_Tabs* o = new Fl_Tabs(0, 23, 395, 384);
      o->labelsize(12);
      { TasksGrp = new Fl_Group(0, 23, 394, 356, "T&asks");
        TasksGrp->labelsize(12);
        { Fl_Table* o = new Fl_Table(0, 23, 394, 354);
          o->color(FL_BACKGROUND2_COLOR);
          o->labelfont(4);
          o->end();
          Fl_Group::current()->resizable(o);
        } // Fl_Table* o
        TasksGrp->end();
      } // Fl_Group* TasksGrp
      { SymbolsGrp = new Fl_Group(0, 25, 394, 350, "&Symbols");
        SymbolsGrp->labelsize(12);
        SymbolsGrp->hide();
        SymbolsGrp->end();
        Fl_Group::current()->resizable(SymbolsGrp);
      } // Fl_Group* SymbolsGrp
      { ObjectsGrp = new Fl_Group(0, 25, 394, 350, "&Objects");
        ObjectsGrp->labelsize(12);
        ObjectsGrp->hide();
        ObjectsGrp->end();
      } // Fl_Group* ObjectsGrp
      { TemplatesGrp = new Fl_Group(0, 25, 394, 350, "Te&mplates");
        TemplatesGrp->labelsize(12);
        TemplatesGrp->hide();
        TemplatesGrp->end();
      } // Fl_Group* TemplatesGrp
      o->end();
    } // Fl_Tabs* o
    FlWindow->end();
  } // Fl_Double_Window* FlWindow
}

AboutWindow::AboutWindow() {
  { FlWindow = new Fl_Double_Window(405, 190, "About OSPASOFT");
    FlWindow->user_data((void*)(this));
    FlWindow->align(Fl_Align(FL_ALIGN_CLIP|FL_ALIGN_INSIDE));
    { Fl_Text_Display* o = new Fl_Text_Display(-2, 15, 15, 25, "OSPASOFT");
      o->box(FL_NO_BOX);
      o->labelfont(1);
      o->labelsize(24);
      o->align(Fl_Align(FL_ALIGN_RIGHT));
    } // Fl_Text_Display* o
    { Fl_Text_Display* o = new Fl_Text_Display(-1, 40, 15, 25, "Programming software for the Open Source PLC Architecture");
      o->box(FL_NO_BOX);
      o->labelsize(12);
      o->textsize(12);
      o->align(Fl_Align(FL_ALIGN_RIGHT));
    } // Fl_Text_Display* o
    { Fl_Text_Display* o = new Fl_Text_Display(-1, 70, 15, 25, "Copyright \302\251 2014 Brian Luft");
      o->box(FL_NO_BOX);
      o->labelsize(12);
      o->textsize(12);
      o->align(Fl_Align(FL_ALIGN_RIGHT));
    } // Fl_Text_Display* o
    { CloseBtn = new Fl_Return_Button(310, 155, 85, 25, "Close");
      CloseBtn->labelsize(12);
    } // Fl_Return_Button* CloseBtn
    { Fl_Text_Display* o = new Fl_Text_Display(-1, 108, 15, 25, "OSPASOFT is based in part on the work of the FLTK project (http://www.fltk.or\
g).");
      o->box(FL_NO_BOX);
      o->labelsize(12);
      o->textsize(12);
      o->align(Fl_Align(136));
    } // Fl_Text_Display* o
    FlWindow->set_modal();
    FlWindow->end();
  } // Fl_Double_Window* FlWindow
}
